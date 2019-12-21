using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators.Domain
{
    public class EntityCodeGenerator : ICodeGenerator
    {
        public string Layer => "Domain";

        public IEnumerable<BaseType> GetListFromModule(Module module)
        {
            return module.Entities;
        }

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("Entities");
        }

        public async Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw, SyntaxGenerator syntaxGenerator)
        {
            if (type is Entity entity)
            {
                var builder = new ClassBuilder(syntaxGenerator, @namespace, entity.Name);
                builder.AddBaseType("Entities");
                entity.GenerateProperties(builder, true);
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
