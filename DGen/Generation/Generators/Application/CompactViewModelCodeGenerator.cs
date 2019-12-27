using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
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
                AddViewModelProperty(registry, @class, property);
            }
            if (viewModel.Target != null)
            {
                foreach (var property in viewModel.Target.Properties)
                {
                    AddViewModelProperty(registry, @class, property);
                }
            }
        }

        private static void AddViewModelProperty(ITypeModelRegistry registry, ClassModel @class, Property property)
        {
            if (property.Type.Type is Aggregate aggregate && aggregate.UniqueIdentifier != null)
            {
                @class.AddViewModelProperty(new Property
                {
                    Name = property.IsCollection ? property.Name : $"{property.Name}{aggregate.UniqueIdentifier.Name}",
                    IsCollection = property.IsCollection,
                    Type = aggregate.UniqueIdentifier.Type
                }.Denormalized(), registry);
            }
            else
            {
                @class.AddViewModelProperty(property.Denormalized(), registry);
            }
        }
    }
}
