using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators.Infrastructure
{
    public class EntityCodeGenerator : ICodeGenerator
    {
        public string Layer => "Infrastructure";

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("Entities");
        }

        public async Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw, SyntaxGenerator syntaxGenerator)
        {
            if (type is Aggregate aggregate)
            {
                var builder = new ClassBuilder(syntaxGenerator, @namespace, aggregate.Name);
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
            return module.Aggregates;
        }
    }
}
