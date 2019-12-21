using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators
{
    public class DbContextCodeGenerator : ICodeGenerator
    {
        public string Layer => "Infrastructure";

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di;
        }

        public async Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw, SyntaxGenerator syntaxGenerator)
        {
            var builder = new ClassBuilder(syntaxGenerator, @namespace, $"{module.Name}DbContext");
            builder.AddBaseType("DbContext");
            await sw.WriteAsync(builder.ToString());
        }

        public string GetFileName(BaseType type)
        {
            return null;
        }

        public string GetFileNameForModule(Module module)
        {
            if(module.Aggregates.Any())
                return $"{module.Name}DbContext.cs";
            return null;
        }

        public IEnumerable<BaseType> GetListFromModule(Module module)
        {
            return new List<BaseType>();
        }
    }
}
