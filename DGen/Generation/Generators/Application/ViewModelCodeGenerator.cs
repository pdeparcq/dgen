using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators.Application
{
    public class ViewModelCodeGenerator : ICodeGenerator
    {
        public string Layer => "Application";

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("ViewModels");
        }

        public async Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw, SyntaxGenerator syntaxGenerator)
        {
            if(type is ViewModel viewModel)
            {
                var builder = new ClassBuilder(syntaxGenerator, @namespace, viewModel.Name);
                await sw.WriteAsync(builder.ToString());
            }
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
