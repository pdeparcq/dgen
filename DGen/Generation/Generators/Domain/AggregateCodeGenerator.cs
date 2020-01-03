using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Domain
{
    public class AggregateCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Domain";

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Aggregates;
        }

        public override string GetTypeName(BaseType type)
        {
            return $"{type.Name}Base";
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is Aggregate aggregate && model is ClassModel @class)
            {
                @class = @class.WithBaseType(SystemTypes.AggregateRoot(@class));

                foreach (var p in aggregate.Properties)
                {
                    @class.AddDomainProperty(p, registry);
                }


                if (aggregate.UniqueIdentifier != null && aggregate.UniqueIdentifier.Type.Resolve(registry) != SystemTypes.Guid)
                {
                    var parameter = new MethodParameter("id", aggregate.UniqueIdentifier.Type.Resolve(registry));

                    var method = @class.AddMethod("GetUniqueIdentifier")
                        .WithReturnType(SystemTypes.Guid)
                        .WithParameters(parameter);

                    var property = aggregate.UniqueIdentifier.Denormalized();
                    if (property.Type.Resolve(registry) == SystemTypes.Guid)
                    {
                        method.WithBody(builder =>
                        {
                            builder.Return(parameter.Property(aggregate.UniqueIdentifier.Type.Type.Properties.First().Name));
                        }).MakeVirtual();
                    }
                }
                

                foreach(var de in aggregate.DomainEvents)
                {
                    if (registry.Resolve(Layer, de) is ClassModel domainEvent)
                    {
                        var parameters = GenerateDomainEventParameters(registry, de, aggregate).ToList();

                        // Add method for publishing domain event
                        @class.AddMethod($"Publish{de.Name}")
                            .WithParameters(parameters.ToArray())
                            .MakeProtected()
                            .WithBody(builder =>
                                {
                                    if (aggregate.UniqueIdentifier != null)
                                    {
                                        if (aggregate.UniqueIdentifier.Type.Resolve(registry) != SystemTypes.Guid)
                                        {
                                            builder.InvokeMethod(
                                                SystemTypes.DomainEventPublishMethodName, 
                                                domainEvent.Construct(
                                                    parameters.ToExpressions(),
                                                    domainEvent.Initializer(SystemTypes.DomainEventAggregateRootIdentifierName, @class.GetMethod("GetUniqueIdentifier").Invoke(parameters.First().Expression)))
                                                );
                                        }
                                        else
                                        {
                                            builder.InvokeMethod(
                                                SystemTypes.DomainEventPublishMethodName,
                                                domainEvent.Construct(
                                                    parameters.ToExpressions(),
                                                    domainEvent.Initializer(SystemTypes.DomainEventAggregateRootIdentifierName, parameters.First().Expression))
                                                );
                                        }
                                    }
                                    else
                                    {
                                        builder.InvokeMethod(SystemTypes.DomainEventPublishMethodName, domainEvent.Construct(parameters.ToExpressions().ToArray()));
                                    }

                                });


                        // Add method for applying domain event
                        var @event = new MethodParameter("@event", domainEvent);
                        @class.AddMethod(SystemTypes.DomainEventApplyMethodName)
                            .WithParameters(@event)
                            .MakeVirtual()
                            .WithBody(builder =>
                            {
                                if (de.Type == DomainEventType.Create)
                                {
                                    builder.AssignProperty(SystemTypes.AggregateRootIdentifierName, @event.Property(SystemTypes.DomainEventAggregateRootIdentifierName));
                                }
                                foreach (var property in de.Properties)
                                {
                                    builder.AssignProperty(property.Name, @event.Property(property.Name));
                                }
                            });

                        if (de.Type == DomainEventType.Create)
                        {
                            @class.AddConstructor()
                                .WithParameters(parameters.ToArray()).WithBody(builder =>
                                {
                                    builder.InvokeMethod($"Publish{de.Name}", parameters.ToExpressions().ToArray());
                                }).MakeProtected();
                        }
                    }
                }
            }
        }

        private static IEnumerable<MethodParameter> GenerateDomainEventParameters(ITypeModelRegistry registry, DomainEvent de, Aggregate aggregate)
        {

            foreach (var property in de.Properties)
            {
                if (property.Type.Type == aggregate && aggregate.UniqueIdentifier != null)
                {
                    yield return new MethodParameter(aggregate.UniqueIdentifier.Name, aggregate.UniqueIdentifier.Type.Resolve(registry));
                }
                else
                {
                    yield return new MethodParameter(property.Name.ToCamelCase(), property.Type.Resolve(registry));
                }
            }
        }
    }
}
