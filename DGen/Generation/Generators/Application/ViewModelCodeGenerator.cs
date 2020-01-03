using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Application
{
    public class ViewModelCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Application";

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.ViewModels.Where(vm => !vm.IsCompact);
        }

        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("ViewModels");
        }

        public override string GetTypeName(BaseType type)
        {
            return $"{type.Name}ViewModel";
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
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
