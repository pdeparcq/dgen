using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta;

namespace DGen.Generation.Generators.Domain
{
    public class EnumerationCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Domain";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Enumerations;
        }

        public string GetTypeName(BaseType type) => type.Name;

        public NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Enumerations");
        }

        public TypeModel PrepareType(NamespaceModel @namespace, BaseType type) => GetNamespace(@namespace).AddEnumeration(GetTypeName(type));

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
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
