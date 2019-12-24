using System.Collections.Generic;
using System.IO;
using DGen.Generation.CodeModel;
using DGen.Meta;

namespace DGen.Generation.Generators.Application
{
    public class ViewModelCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Application";

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("ViewModels");
        }

        public void Visit(Module module, NamespaceModel @namespace)
        {
            
        }

        public void Visit(BaseType type, NamespaceModel @namespace)
        {
            if (type is ViewModel viewModel)
            {
                @namespace = @namespace.AddNamespace("ViewModels");
                @namespace.AddClass($"{viewModel.Name}ViewModel");
            }
        }

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.ViewModels;
        }
    }
}
