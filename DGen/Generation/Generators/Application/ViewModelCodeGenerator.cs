using System.Collections.Generic;
using System.IO;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis;

namespace DGen.Generation.Generators.Application
{
    public class ViewModelCodeGenerator : ICodeGenerator
    {
        public string Layer => "Application";

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("ViewModels");
        }

        public SyntaxNode Generate(CodeGenerationContext context)
        {
            if(context.Type is ViewModel viewModel)
            {
                var builder = new ClassBuilder(context.SyntaxGenerator, context.Namespace, viewModel.Name);
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
            return module.ViewModels;
        }
    }
}
