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
                    if(de.Type == DomainEventType.Create)
                    {
                        var constructor = @class.AddConstructor()
                            .WithParameters(de.Properties.Select(p => new MethodParameter(p.Name.ToCamelCase(), p.Type.Resolve(registry))).ToArray());

                        foreach(var param in constructor.Parameters)
                        {
                            constructor.Assign(param.Expression, param.Expression);
                        }
                    }

                    @class.AddMethod("Apply")
                        .WithParameters(new MethodParameter("@event", registry.Resolve(Layer, de)));
                }
            }
        }
    }
}
