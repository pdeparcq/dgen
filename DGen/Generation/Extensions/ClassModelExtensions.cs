using DGen.Generation.CodeModel;
using DGen.Generation.Generators;
using DGen.Meta;
using System.Linq;

namespace DGen.Generation.Extensions
{
    public static class ClassModelExtensions
    {
        public static void AddDomainProperty(this ClassModel @class, Property property, ITypeModelRegistry registry)
        {
            var propertyName = property.Name;
            TypeModel propertyType;

            if (property.Type.Type is Aggregate aggregate && aggregate.UniqueIdentifier != null)
            {
                if (!property.IsCollection)
                    propertyName = $"{propertyName}{aggregate.UniqueIdentifier.Name}";
                propertyType = aggregate.UniqueIdentifier.Type.Resolve(registry);
            }
            else
            {
                propertyType = property.Type.Resolve(registry);
            }

            if (property.IsCollection)
                propertyType = SystemTypes.GenericList(propertyType);

            @class.AddProperty(propertyName, propertyType)
                .WithDescription(property.Description)
                .MakeReadOnly();
        }

        public static void AddViewModelProperty(this ClassModel @class, Property property, ITypeModelRegistry registry)
        {
            TypeModel propertyType;

            if (property.Type.SystemType != null)
            {
                propertyType = SystemTypes.Parse(property.Type.SystemType);
            }
            else if (!(property.Type.Type is Enumeration))
            {
                var viewModel = registry.GetAllBaseTypes<ViewModel>("Application").SingleOrDefault(vm => vm.IsCompact && vm.Target == property.Type.Type);
                propertyType = registry.Resolve("Application", viewModel);
            }
            else
            {
                propertyType = property.Type.Resolve(registry);
            }

            if (property.IsCollection)
                propertyType = SystemTypes.GenericList(propertyType);

            @class.AddProperty(property.Name, propertyType)
                .WithDescription(property.Description);
        }
    }
}
