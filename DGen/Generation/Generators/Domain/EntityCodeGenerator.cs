using System.Collections.Generic;
using System.IO;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis;

namespace DGen.Generation.Generators.Domain
{
    public class EntityCodeGenerator : ICodeGenerator
    {
        public string Layer => "Domain";

        public string Namespace => null;

        public IEnumerable<BaseType> GetTypesFromModule(Module module)
        {
            return module.Entities;
        }

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("Entities");
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
            if (context.Type is Entity entity)
            {
                var builder = new ClassBuilder(context.SyntaxGenerator, context.Namespace, entity.Name);
                builder.AddBaseType("Entity");
                entity.GenerateProperties(builder, true);
                return builder.Build();
            }
            return null;
        }
    }
}
