using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators.Domain
{
    public class DomainEventCodeGenerator : ICodeGenerator
    {
        public string Layer => "Domain";

        public string Namespace => "DomainEvents";

        public SyntaxNode Generate(CodeGenerationContext context)
        {
            if (context.Type is DomainEvent domainEvent)
            {
                var builder = new ClassBuilder(context.SyntaxGenerator, context.Namespace, domainEvent.Name);
                builder.AddBaseType("DomainEvent");
                domainEvent.GenerateProperties(builder);
                return builder.Build();
            }
            return null;
        }

        public IEnumerable<BaseType> GetTypesFromModule(Module module)
        {
            return module.DomainEvents;
        }

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("DomainEvents");
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
