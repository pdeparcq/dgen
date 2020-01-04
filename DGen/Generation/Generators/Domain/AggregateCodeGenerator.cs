using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

                foreach(var domainEvent in aggregate.DomainEvents)
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

                foreach(var command in aggregate.Module.GetTypes<Command>().Where(c => c.DomainEvent != null && c.DomainEvent.Aggregate == aggregate))
                {
                    var parameters = GenerateDomainEventParameters(registry, command.DomainEvent, aggregate, command.DomainEvent.Type == DomainEventType.Create).ToList();

                    var validatorMethod = @class.AddMethod($"Validate{command.Name}")
                        .WithParameters(parameters.ToArray())
                        .MakeProtected()
                        .MakeVirtual();

                    if (command.DomainEvent.Type == DomainEventType.Create)
                    {
                        @class.AddConstructor()
                            .WithParameters(parameters.ToArray())
                            .WithBody(builder =>
                            {
                                builder.InvokeMethod(validatorMethod.Name, parameters.ToExpressions().ToArray());
                                builder.InvokeMethod($"Publish{command.DomainEvent.Name}", parameters.ToExpressions().ToArray());
                            });
                    }
                    else
                    {
                        @class.AddMethod(command.MethodName)
                            .WithParameters(parameters.ToArray())
                            .WithBody(builder =>
                            {
                                var publishParameters = parameters.ToExpressions().ToList();
                                publishParameters.Insert(0, @class.GetProperty(SystemTypes.AggregateRootIdentifierName).Expression);

                                builder.InvokeMethod(validatorMethod.Name, parameters.ToExpressions().ToArray());
                                builder.InvokeMethod($"Publish{command.DomainEvent.Name}", publishParameters.ToArray());
                            })
                            .MakeVirtual();
                    }
                }
            }
        }

        private static void BuildDomainEventApply(BodyBuilder builder, DomainEvent de, MethodParameter @event)
        {
            if (de.Type == DomainEventType.Create)
            {
                builder.AssignProperty(SystemTypes.AggregateRootIdentifierName, @event.Property(SystemTypes.DomainEventAggregateRootIdentifierName));
            }
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
        }

        private static IEnumerable<MethodParameter> GenerateDomainEventParameters(ITypeModelRegistry registry, DomainEvent de, Aggregate aggregate, bool includeUniqueId = true)
        {
            if(aggregate.UniqueIdentifier != null && includeUniqueId)
            {
                yield return new MethodParameter(aggregate.UniqueIdentifier.Name.ToCamelCase(), aggregate.UniqueIdentifier.Type.Resolve(registry));
            }
            foreach (var property in de.Properties)
            {
                if (property.Type.Type is Aggregate referencedAggregate && referencedAggregate.UniqueIdentifier != null)
                {
                    yield return new MethodParameter($"{property.Name.ToCamelCase()}{referencedAggregate.UniqueIdentifier.Name}", referencedAggregate.UniqueIdentifier.Type.Resolve(registry));
                }
                else
                {
                    yield return new MethodParameter(property.Name.ToCamelCase(), property.Type.Resolve(registry));
                }
            }
        }
    }
}
