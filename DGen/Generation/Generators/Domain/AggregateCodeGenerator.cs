using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;

namespace DGen.Generation.Generators.Domain
{
    public class AggregateCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Domain";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Aggregates;
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is Aggregate aggregate && model is ClassModel @class)
            {
                @class = @class.WithBaseType(SystemTypes.AggregateRoot(@class));

                foreach (var p in aggregate.Properties)
                {
                    @class.AddDomainProperty(p, registry);
                }

                foreach(var de in aggregate.DomainEvents)
                {
                    var domainEventType = registry.Resolve(Layer, de) as ClassModel;
                    var parameters = GenerateDomainEventParameters(registry, de, aggregate).ToList();

                    @class.AddMethod($"Publish{de.Name}")
                        .WithParameters(parameters.ToArray());

                    @class.AddMethod("Apply")
                        .WithParameters(new MethodParameter("@event", domainEventType));

                    if (de.Type == DomainEventType.Create)
                    {
                        @class.AddConstructor()
                            .WithParameters(parameters.ToArray());
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
