using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Domain
{
    public class EnumerationCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Domain";

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Enumerations;
        }

        public override string GetTypeName(BaseType type) => type.Name;

        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Enumerations");
        }

        public override TypeModel PrepareType(NamespaceModel @namespace, BaseType type) => GetNamespace(@namespace).AddEnumeration(GetTypeName(type));

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is Enumeration e && model is EnumerationModel enumeration)
            {
                foreach (var literal in e.Literals)
                {
                    enumeration.AddLiteral(literal);
                }
            }
        }
    }
}
