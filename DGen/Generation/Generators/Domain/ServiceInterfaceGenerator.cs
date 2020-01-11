using DGen.Generation.CodeModel;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using System.Collections.Generic;

namespace DGen.Generation.Generators.Domain
{
    public class ServiceInterfaceGenerator : LayerCodeGenerator
    {
        public override string Layer => "Domain";

        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Services");
        }

        public override string GetTypeName(BaseType type)
        {
            return $"I{type.Name}";
        }

        public override TypeModel PrepareType(NamespaceModel @namespace, BaseType type)
        {
            return GetNamespace(@namespace).AddInterface(GetTypeName(type));
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is Service service && model is InterfaceModel @interface)
            {
                //TODO
            }
        }

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.GetTypes<Service>();
        }
    }
}
