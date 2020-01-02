using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                    if (property.Type.SystemType != null && SystemTypes.Parse(property.Type.Name) == SystemTypes.Guid)
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
                                    builder.InvokeMethod(SystemTypes.DomainEventPublishMethodName,
                                        domainEvent.Construct(parameters.ToExpressions(),
                                            CreateDomainEventInitializer(aggregate, @class, registry,
                                                parameters.FirstOrDefault())));
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
                                    builder.InvokeMethod($"Publish{de.Name}", parameters.ToExpressions());
                                }).MakeProtected();
                        }
                    }
                }
            }
        }

        private static (string Name, ExpressionSyntax Expression)[] CreateDomainEventInitializer(Aggregate aggregate, ClassModel @class, ITypeModelRegistry registry, MethodParameter parameter)
        {
            if (aggregate.UniqueIdentifier != null)
            {
                var type = aggregate.UniqueIdentifier.Type.Resolve(registry);

                if (type != SystemTypes.Guid)
                {
                    return new[]
                    {
                        (Name: SystemTypes.DomainEventAggregateRootIdentifierName, Expression: @class.GetMethod("GetUniqueIdentifier").Invoke(parameter.Expression) )
                    };
                }

                return new[]
                {
                    (Name: SystemTypes.DomainEventAggregateRootIdentifierName, parameter.Expression )
                };
            }

            return null;
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
