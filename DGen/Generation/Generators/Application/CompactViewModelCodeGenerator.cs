using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Meta;

namespace DGen.Generation.Generators.Application
{
    public class CompactViewModelCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Application";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.ViewModels.Where(vm => vm.IsCompact);
        }

        public NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("ViewModels").AddNamespace("Compact");
        }

        public string GetTypeName(BaseType type)
        {
            return $"Compact{type.Name}ViewModel";
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
                AddViewModelProperty(registry, @class, property.Denormalized());
            }
            if (viewModel.Target != null)
            {
                foreach (var property in viewModel.Target.Properties)
                {
                    AddViewModelProperty(registry, @class, property.Denormalized());
                }
            }
        }

        private void AddViewModelProperty(ITypeModelRegistry registry, ClassModel @class, Property property)
        {
            TypeModel propertyType;

            if (property.Type.SystemType != null)
            {
                propertyType = SystemTypes.Parse(property.Type.SystemType);
            }
            else if (!(property.Type.Type is Enumeration))
            {
                propertyType = registry.Resolve("Domain", property.Type.Type);
            }
            else
            {
                propertyType = registry.Resolve("Domain", property.Type.Type);
            }

            if (property.IsCollection)
                propertyType = SystemTypes.GenericList(propertyType);

            @class.AddProperty(property.Name, propertyType);
        }
    }
}
