using System.Collections.Generic;
using DGen.Generation.CodeModel;
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

        public void Visit(Module module, NamespaceModel @namespace)
        {
            
        }

        public void Visit(BaseType type, NamespaceModel @namespace)
        {
            if (type is Value value)
            {
                @namespace.AddClass($"{value.Name}");
            }
        }
    }
}
