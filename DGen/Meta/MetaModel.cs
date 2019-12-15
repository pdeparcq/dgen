using System.Collections.Generic;

namespace DGen.Meta
{
    public class MetaModel
    {
        public List<Service> Services { get; set; }
    }

    public class Module
    {
        public Module ParentModule { get; set; }
        public string Name { get; set; }
        public string FullName => ParentModule == null ? Name : $"{ParentModule.FullName}.{Name}";
        public List<Module> Modules { get; set; }
        public List<Aggregate> Aggregates { get; set; }
        public List<Entity> Entities { get; set; }
        public List<Value> Values { get; set; }
    }

    public class Property
    {
        public bool IsIdentifier { get; set; }
        public string Name { get; set; }
        public string SystemType { get; set; }
        public BaseType Type { get; set; }
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
        Delete
    }

    public class DomainEvent : BaseType
    {
        public Aggregate Aggregate { get; set; }
        public DomainEventType Type { get; set; }
    }

    public class Value : BaseType
    {
    }

    public class Entity : BaseType
    {
    }

    public class Aggregate : Entity
    {
        public List<DomainEvent> DomainEvents { get; set; }
    }

    
}
