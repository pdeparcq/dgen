using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta;

namespace DGen.Generation.Generators.Application
{
    public class ViewModelCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Application";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.ViewModels;
        }

        public NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("ViewModels");
        }

        public string GetTypeName(BaseType type)
        {
            if(type is ViewModel viewModel && viewModel.IsCompact)
            {
                return $"Compact{viewModel.Name}ViewModel";
            }
            return $"{type.Name}ViewModel";
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is ViewModel viewModel && model is ClassModel @class)
            {
                GenerateViewModel(registry, viewModel, @class, viewModel.IsCompact);
            }
        }

        private void GenerateViewModel(ITypeModelRegistry registry, ViewModel viewModel, ClassModel @class, bool compact = false)
        {
            foreach (var property in viewModel.Properties)
            {
                AddViewModelProperty(registry, @class, property.Denormalized());
            }
            if (viewModel.Target != null)
            {
                foreach (var property in viewModel.Target.Properties)
                {
                    if(compact && property.Type.Type is Aggregate aggregate && aggregate.UniqueIdentifier != null)
                    {
                        AddViewModelProperty(registry, @class, new Property
                        {
                            Name = property.IsCollection ? property.Name : $"{property.Name}{aggregate.UniqueIdentifier.Name}",
                            IsCollection = property.IsCollection,
                            Type = aggregate.UniqueIdentifier.Type
                        }.Denormalized());
                    }
                    else
                    {
                        AddViewModelProperty(registry, @class, property.Denormalized());
                    }    
                }
            }
        }

        private void AddViewModelProperty(ITypeModelRegistry registry, ClassModel @class, Property property)
        {
            TypeModel propertyType;

            if(property.Type.SystemType != null)
            {
                propertyType = SystemTypes.Parse(property.Type.SystemType);
            }
            else if(!(property.Type.Type is Enumeration))
            {
                propertyType = GetCompactViewModelType(registry, @class, property.Type.Type);
            }
            else
            {
                propertyType = registry.Resolve("Domain", property.Type.Type);
            }

            if (property.IsCollection)
                propertyType = SystemTypes.GenericList(propertyType);

            @class.AddProperty(property.Name, propertyType);
        }

        private TypeModel GetCompactViewModelType(ITypeModelRegistry registry, ClassModel @class, BaseType propertyBaseType)
        {
            var typeName = $"Compact{propertyBaseType.Name}ViewModel";
            var propertyType = registry.Resolve(Layer, propertyBaseType, typeName);
            if (propertyType == null)
            {
                propertyType = @class.Namespace.AddClass(typeName);
                registry.Register(Layer, propertyBaseType, propertyType);
                GenerateViewModel(registry, new ViewModel { Target = propertyBaseType }, propertyType as ClassModel, true);
            }
            return propertyType;
        }
    }
}
