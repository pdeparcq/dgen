using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DGen.Meta.MetaModel
{
    public class Property : System.IEquatable<Property>
    {
        public bool IsIdentifier { get; set; }
        public bool IsCollection { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PropertyType Type { get; set; }

        public bool Equals([AllowNull] Property other)
        {
            if (other == null)
                return false;

            return other.Name == Name && other.Type.Equals(Type) && other.IsCollection == IsCollection && other.IsIdentifier == IsIdentifier;
        }

        public Property Denormalized()
        {
            var type = Type;

            if(type.SystemType == null && type.Type.Properties.Count == 1 && !type.Type.Properties.First().IsCollection && type.Type.Properties.First().Type.SystemType != null)
            {
                type = type.Type.Properties.First().Type;
            }

            return new Property
            {
                IsIdentifier = IsIdentifier,
                IsCollection = IsCollection,
                Name = Name,
                Description = Description,
                Type = type
            };
        }
    }
}