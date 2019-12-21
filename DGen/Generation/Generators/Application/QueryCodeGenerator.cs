using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators.Application
{
    public class QueryCodeGenerator : ICodeGenerator
    {
        public string Layer => "Application";

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("Queries");
        }

        public async Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw, SyntaxGenerator syntaxGenerator)
        {
            if (type is Query query)
            {
                var builder = new ClassBuilder(syntaxGenerator, @namespace, query.Name);
                if(query.Result != null)
                {
                    builder.AddBaseType(query.IsCollection ? $"Query<IEnumerable<{query.Result.Name}>>" : $"Query<{query.Result.Name}>");
                }
                else
                {
                    builder.AddBaseType("Query");
                }
                query.GenerateProperties(builder);
                await sw.WriteAsync(builder.ToString());
            }
        }

        public string GetFileName(BaseType type)
        {
            return $"{type.Name}.cs";
        }

        public string GetFileNameForModule(Module module)
        {
            return null;
        }

        public IEnumerable<BaseType> GetListFromModule(Module module)
        {
            return module.Queries;
        }
    }
}
