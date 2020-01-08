using DGen.Generation.CodeModel;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Extensions
{
    public static class PropertyExtensions
    {
        public static string GetDomainName(this Property property)
        {

            if (property.Type.Type is Aggregate aggregate && aggregate.UniqueIdentifier != null)
            {
                if (!property.IsCollection)
                    return $"{property.Name}{aggregate.UniqueIdentifier.Name}";
            }
            return property.Name;
        }

        public static TypeModel GetDomainType(this Property property, ITypeModelRegistry registry)
        {
            TypeModel propertyType;

            if (property.Type.Type is Aggregate aggregate && aggregate.UniqueIdentifier != null)
            {
                propertyType = aggregate.UniqueIdentifier.Type.Resolve(registry);
            }
            else
            {
                propertyType = property.Type.Resolve(registry);
            }

            if (property.IsCollection)
                return SystemTypes.GenericList(propertyType);

            return propertyType;
        }
    }
}
