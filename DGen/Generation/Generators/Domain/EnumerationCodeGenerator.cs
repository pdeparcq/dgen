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

        public void Visit(Module module, NamespaceModel @namespace)
        {
            
        }

        public void Visit(BaseType type, NamespaceModel @namespace)
        {
            if (type is Enumeration e)
            {
                @namespace = @namespace.AddNamespace("Enumerations");
                var enumeration = @namespace.AddEnumeration($"{e.Name}");

                foreach(var literal in e.Literals)
                {
                    enumeration.AddLiteral(literal);
                }
            }
        }
    }
}
