using System.Collections.Generic;
using System.IO;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis;

namespace DGen.Generation.Generators.Infrastructure
{
    public class EntityCodeGenerator : ICodeGenerator
    {
        public string Layer => "Infrastructure";

        public string Namespace => "Entities";

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("Entities");
        }

        public SyntaxNode Generate(CodeGenerationContext context)
        {
            if (context.Type is Aggregate aggregate)
            {
                var builder = new ClassBuilder(context.SyntaxGenerator, context.Namespace, aggregate.Name);
                return builder.Build();
            }
            return null;
        }

        public string GetFileName(BaseType type)
        {
            return $"{type.Name}.cs";
        }

        public string GetFileNameForModule(Module module)
        {
            return null;
        }

        public IEnumerable<BaseType> GetTypesFromModule(Module module)
        {
            return module.Aggregates;
        }
    }
}
