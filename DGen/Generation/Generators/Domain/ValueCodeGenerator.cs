using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators.Domain
{
    public class ValueCodeGenerator : ICodeGenerator
    {
        public string Layer => "Domain";

        public IEnumerable<BaseType> GetTypesFromModule(Module module)
        {
            return module.Values;
        }

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("ValueObjects");
        }

        public async Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw, SyntaxGenerator syntaxGenerator)
        {
            if (type is Value value)
            {
                var builder = new ClassBuilder(syntaxGenerator, @namespace, value.Name);
                builder.AddBaseType("ValueObjects");
                value.GenerateProperties(builder, true);
                await sw.WriteAsync(builder.ToString());
            }
        }

        public string GetFileNameForModule(Module module)
        {
            return null;
        }

        public string GetFileName(BaseType type)
        {
            return $"{type.Name}.cs";
        }
    }
}
