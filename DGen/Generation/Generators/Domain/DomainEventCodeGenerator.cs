using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators.Domain
{
    public class DomainEventCodeGenerator : ICodeGenerator
    {
        public string Layer => "Domain";

        public IEnumerable<BaseType> GetTypesFromModule(Module module)
        {
            return module.DomainEvents;
        }

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("DomainEvents");
        }

        public async Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw, SyntaxGenerator syntaxGenerator)
        {
            if (type is DomainEvent domainEvent)
            {
                var builder = new ClassBuilder(syntaxGenerator, @namespace, domainEvent.Name);
                builder.AddBaseType("DomainEvent");
                domainEvent.GenerateProperties(builder);
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
