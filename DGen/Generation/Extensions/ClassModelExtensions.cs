using DGen.Generation.CodeModel;
using DGen.Generation.Generators;
using DGen.Meta;

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

            @class.AddProperty(propertyName, propertyType).MakeReadOnly();
        }
    }
}
