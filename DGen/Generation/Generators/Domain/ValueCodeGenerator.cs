using System.Collections.Generic;
using System.IO;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis;

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

        public string GetFileNameForModule(Module module)
        {
            return null;
        }

        public string GetFileName(BaseType type)
        {
            return $"{type.Name}.cs";
        }

        public SyntaxNode Generate(CodeGenerationContext context)
        {
            if (context.Type is Value value)
            {
                var builder = new ClassBuilder(context.SyntaxGenerator, context.Namespace, value.Name);
                builder.AddBaseType("ValueObject");
                value.GenerateProperties(builder, true);
                return builder.Build();
            }
            return null;
        }
    }
}
