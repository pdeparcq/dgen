using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Domain
{
    public class EntityCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Domain";

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.GetTypes<Entity>();
        }

        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Entities");
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
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
