using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;

namespace DGen.Generation.Generators.Domain
{
    public class ValueCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Domain";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Values;
        }

        public TypeModel PrepareType(BaseType type, NamespaceModel @namespace)
        {
            @namespace = @namespace.AddNamespace("ValueObjects");
            return @namespace.AddClass($"{type.Name}");
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is Value value && model is ClassModel @class)
            {
                foreach (var p in value.Properties)
                {
                    @class.AddDomainProperty(p, registry);
                }
            }
        }
    }
}
