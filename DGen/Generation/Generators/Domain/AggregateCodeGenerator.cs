using System.Collections.Generic;
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

        public TypeModel PrepareType(BaseType type, NamespaceModel @namespace)
        {
            return @namespace.AddClass($"{type.Name}");
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is Aggregate aggregate && model is ClassModel @class)
            {
                foreach (var p in aggregate.Properties)
                {
                    @class.AddDomainProperty(p, registry);
                }
            }
        }
    }
}
