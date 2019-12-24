using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta;

namespace DGen.Generation.Generators.Application
{
    public class QueryCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Application";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Queries;
        }

        public void Visit(Module module, NamespaceModel @namespace)
        {
            
        }

        public void Visit(BaseType type, NamespaceModel @namespace)
        {
            if (type is Query query)
            {
                @namespace = @namespace.AddNamespace("Queries");
                @namespace.AddClass($"{query.Name}");
            }
        }
    }
}
