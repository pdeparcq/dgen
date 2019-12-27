using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;

namespace DGen.Generation.Generators.Application
{
    public class ViewModelCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Application";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.ViewModels.Where(vm => !vm.IsCompact);
        }

        public NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("ViewModels");
        }

        public string GetTypeName(BaseType type)
        {
            return $"{type.Name}ViewModel";
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is ViewModel viewModel && model is ClassModel @class)
            {
                GenerateViewModel(registry, viewModel, @class);
            }
        }

        private void GenerateViewModel(ITypeModelRegistry registry, ViewModel viewModel, ClassModel @class)
        {
            foreach (var property in viewModel.Properties)
            {
                @class.AddViewModelProperty(property.Denormalized(), registry);
            }
            if (viewModel.Target != null)
            {
                foreach (var property in viewModel.Target.Properties)
                {
                    @class.AddViewModelProperty(property.Denormalized(), registry);
                }
            }
        }
    }
}
