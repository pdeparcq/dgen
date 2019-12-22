using System.Collections.Generic;
using System.IO;
using System.Linq;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis;

namespace DGen.Generation.Generators.Infrastructure
{
    public class DbContextCodeGenerator : ICodeGenerator
    {
        public string Layer => "Infrastructure";

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di;
        }

        public SyntaxNode Generate(CodeGenerationContext context)
        {
            var builder = new ClassBuilder(context.SyntaxGenerator, context.Namespace, $"{context.Module.Name}DbContext");
            builder.AddBaseType("DbContext");
            return builder.Build();
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

        public IEnumerable<BaseType> GetTypesFromModule(Module module)
        {
            return new List<BaseType>();
        }
    }
}
