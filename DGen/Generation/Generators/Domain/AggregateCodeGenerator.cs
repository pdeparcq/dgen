using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Domain
{
    public class AggregateCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Domain";

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.GetTypes<Aggregate>();
        }

        public override string GetTypeName(BaseType type)
        {
            return $"{type.Name}";
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

                    var method = @class.AddMethod("ToGuid")
                        .WithReturnType(SystemTypes.Guid)
                        .WithParameters(parameter);

                    var property = aggregate.UniqueIdentifier.Denormalized();
                    if (property.Type.Resolve(registry) == SystemTypes.Guid)
                    {
                        method.WithBody(builder =>
                        {
                            builder.Return(parameter.Property(aggregate.UniqueIdentifier.Type.Type.Properties.First().Name));
                        }).MakeProtected().MakeVirtual();
                    }

                    @class.AddMethod("FromGuid")
                        .WithReturnType(aggregate.UniqueIdentifier.Type.Resolve(registry))
                        .WithParameters(new MethodParameter("id", SystemTypes.Guid))
                        .WithBody(builder => { builder.ThrowNotImplemented(); })
                        .MakeProtected().MakeVirtual();
                }

                foreach(var domainEvent in aggregate.Module.GetTypes<DomainEvent>().Where(e => e.Aggregate == aggregate))
                {
                    if (registry.Resolve(Layer, domainEvent) is ClassModel domainEventClass)
                    {
                        var parameters = GenerateDomainEventParameters(registry, domainEvent, aggregate).ToList();

                        // Add method for publishing domain event
                        @class.AddMethod($"Publish{domainEvent.Name}")
                            .WithParameters(parameters.ToArray())
                            .MakeProtected()
                            .MakeVirtual()
                            .WithBody(builder =>
                            {
                                BuildDomainEventPublisher(registry, builder, aggregate, @class, domainEventClass, parameters);
                            });

                        // Add method for applying domain event
                        var @event = new MethodParameter("@event", domainEventClass);
                        @class.AddMethod(SystemTypes.DomainEventApplyMethodName)
                            .WithParameters(@event)
                            .MakeVirtual()
                            .WithBody(builder =>
                            {
                                BuildDomainEventApply(builder, domainEvent, @event);
                            });
                    }       
                }

                foreach(var metaMethod in aggregate.Methods)
                {
                    var method = @class.AddMethod(metaMethod.Name)
                        .WithParameters(metaMethod.Parameters.Select(p => new MethodParameter(p.Name, p.Type.Resolve(registry))).ToArray());
                    
                    if(metaMethod.Return != null)
                    {
                        method = method.WithReturnType(metaMethod.Return.Type.Resolve(registry));
                    }
                }
            }
        }

        private static void BuildDomainEventApply(BodyBuilder builder, DomainEvent de, MethodParameter @event)
        {
            builder.AssignProperty(SystemTypes.AggregateRootIdentifierName, @event.Property(SystemTypes.DomainEventAggregateRootIdentifierName));

            foreach (var property in de.Properties)
            {
                builder.AssignProperty(property.Name, @event.Property(property.Name));
            }
        }

        private static void BuildDomainEventPublisher(ITypeModelRegistry registry, BodyBuilder builder, Aggregate aggregate, ClassModel @class, ClassModel domainEvent, List<MethodParameter> parameters)
        {
            if (aggregate.UniqueIdentifier != null)
            {
                if (aggregate.UniqueIdentifier.Type.Resolve(registry) != SystemTypes.Guid)
                {
                    builder.InvokeMethod(
                        SystemTypes.DomainEventPublishMethodName,
                        domainEvent.Construct(
                            parameters.ToExpressions(),
                            domainEvent.Initializer(SystemTypes.DomainEventAggregateRootIdentifierName, @class.GetMethod("ToGuid").Invoke(parameters.First().Expression)))
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
        }

        private static IEnumerable<MethodParameter> GenerateDomainEventParameters(ITypeModelRegistry registry, DomainEvent de, Aggregate aggregate, bool includeUniqueId = true)
        {
            if(aggregate.UniqueIdentifier != null && includeUniqueId)
            {
                yield return new MethodParameter(aggregate.UniqueIdentifier.Name.ToCamelCase(), aggregate.UniqueIdentifier.Type.Resolve(registry));
            }
            foreach (var property in de.Properties)
            {
                yield return new MethodParameter(property.GetDomainName().ToCamelCase(), property.GetDomainType(registry));
            }
        }
    }
}
