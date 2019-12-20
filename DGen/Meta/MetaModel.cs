using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DGen.Meta
{
    public class MetaModel
    {
        public string Name { get; set; }
        public List<Service> Services { get; set; }
    }

    public class Module
    {
        public Module()
        {
            Modules = new List<Module>();
            Aggregates = new List<Aggregate>();
            Entities = new List<Entity>();
            Values = new List<Value>();
            DomainEvents = new List<DomainEvent>();
            Enumerations = new List<Enumeration>();
            Queries = new List<Query>();
            ViewModels = new List<ViewModel>();
        }

        public Module ParentModule { get; set; }
        public string Name { get; set; }
        public List<Module> Modules { get; set; }
        public List<Aggregate> Aggregates { get; set; }
        public List<Entity> Entities { get; set; }
        public List<Value> Values { get; set; }
        public List<DomainEvent> DomainEvents { get; set; }
        public List<Enumeration> Enumerations { get; set; }
        public List<Query> Queries { get; set; }
        public List<ViewModel> ViewModels { get; set; }
    }

    public class PropertyType : System.IEquatable<PropertyType>
    {
        public string SystemType { get; set; }
        public BaseType Type { get; set; }

        public string Name => SystemType ?? Type?.Name;

        public bool Equals([AllowNull] PropertyType other)
        {
            if (other == null)
                return false;

            return (other.SystemType == SystemType) && (other.Type?.Name == Type?.Name);
        }
    }

    public class Property : System.IEquatable<Property>
    {
        public bool IsIdentifier { get; set; }
        public bool IsCollection { get; set; }
        public string Name { get; set; }
        public PropertyType Type { get; set; }

        public bool Equals([AllowNull] Property other)
        {
            if (other == null)
                return false;

            return other.Name == Name && other.Type.Equals(Type);
        }
    }

    public class Service : Module
    {

    }

    public abstract class BaseType
    {
        public Module Module { get; set; }
        public string Name { get; set; }
        public List<Property> Properties { get; set; }
    }


    public enum DomainEventType
    {
        Create,
        Update,
        Add,
        Remove,
        Delete
    }

    public class DomainEvent : BaseType
    {
        public Aggregate Aggregate { get; set; }
        public DomainEventType Type { get; set; }
    }

    public class Enumeration : BaseType
    {
        public List<string> Literals { get; set; }
    }

    public class Value : BaseType
    {
    }

    public class Entity : BaseType
    {
        public Property UniqueIdentifier => Properties?.FirstOrDefault(p => p.IsIdentifier);
    }

    public class Aggregate : Entity
    {
        public List<DomainEvent> DomainEvents { get; set; }
    }

    public class ViewModel : BaseType
    {
        BaseType Target { get; set; }
        public bool IsCompact { get; set; }
    }

    public class Query : BaseType
    {
        ViewModel Result { get; set; }
        public bool IsCollection { get; set; }
    }
}
