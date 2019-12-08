using System.Collections.Generic;

namespace DGen.Test.Meta
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
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Service : Module
    {

    }

    public class Entity
    {
        public string Name { get; set; }
        public List<Property> Properties { get; set; }
    }

    public class Aggregate : Entity
    {

    }

    public class Value
    {
        public string Name { get; set; }
    }
}
