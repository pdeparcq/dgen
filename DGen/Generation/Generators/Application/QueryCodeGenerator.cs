using System.Collections.Generic;
using System.IO;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis;

namespace DGen.Generation.Generators.Application
{
    public class QueryCodeGenerator : ICodeGenerator
    {
        public string Layer => "Application";

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("Queries");
        }

        public SyntaxNode Generate(CodeGenerationContext context)
        {
            if (context.Type is Query query)
            {
                var builder = new ClassBuilder(context.SyntaxGenerator, context.Namespace, query.Name);
                builder.AddBaseType(query.IsCollection ? $"IQuery<IEnumerable<{query.Result.Name}ViewModel>>" : $"IQuery<{query.Result.Name}ViewModel>");
                query.GenerateProperties(builder, false, true);
                return builder.Build();
            }
            return null;
        }

        public string GetFileName(BaseType type)
        {
            if (type is Query query && query.Result != null)
                return $"{query.Name}.cs";
            return null;
        }

        public string GetFileNameForModule(Module module)
        {
            return null;
        }

        public IEnumerable<BaseType> GetTypesFromModule(Module module)
        {
            return module.Queries;
        }
    }
}
