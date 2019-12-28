using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;

namespace DGen.Generation.Generators.Domain
{
    public class EntityCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Domain";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Entities;
        }

        public NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Entities");
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is Entity entity && model is ClassModel @class)
            {
                foreach (var p in entity.Properties)
                {
                    @class.AddDomainProperty(p, registry);
                }
            }
        }
    }
}
