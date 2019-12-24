using System.Collections.Generic;
using System.IO;
using DGen.Generation.CodeModel;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis;

namespace DGen.Generation.Generators.Application
{
    public class QueryHandlerCodeGenerator : ICodeModelGenerator
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
                @namespace.AddClass($"{query.Name}QueryHandler");
            }
        }
    }
}
