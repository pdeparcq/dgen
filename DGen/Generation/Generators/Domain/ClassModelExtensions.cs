using DGen.Generation.CodeModel;
using DGen.Meta;

namespace DGen.Generation.Generators.Domain
{
    public static class ClassModelExtensions
    {
        public static void AddDomainProperty(this ClassModel @class, Property property)
        {
            var propertyName = property.Name;
            TypeModel propertyType;

            if (property.Type.Type is Aggregate aggregate && aggregate.UniqueIdentifier != null)
            {
                if (!property.IsCollection)
                    propertyName = $"{propertyName}{aggregate.UniqueIdentifier.Name}";
                propertyType = SystemTypes.Parse(aggregate.UniqueIdentifier.Type.Name);
            }
            else
            {
                propertyType = SystemTypes.Parse(property.Type.Name);
            }

            if (property.IsCollection)
                propertyType = SystemTypes.GenericList(propertyType);

            @class.AddProperty(propertyName, propertyType).MakeReadOnly();
        }
    }
}
