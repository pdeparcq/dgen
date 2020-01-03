using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Domain
{
    public class ValueCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Domain";

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Values;
        }

        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("ValueObjects");
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
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
