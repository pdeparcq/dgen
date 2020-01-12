using System.Diagnostics.CodeAnalysis;

namespace DGen.Meta.MetaModel
{
    public class MetaType : System.IEquatable<MetaType>
    {
        public string SystemType { get; set; }
        public BaseType Type { get; set; }

        public string Name => SystemType ?? Type?.Name;

        public bool Equals([AllowNull] MetaType other)
        {
            if (other == null)
                return false;

            return (other.SystemType == SystemType) && (other.Type?.Name == Type?.Name);
        }
    }
}