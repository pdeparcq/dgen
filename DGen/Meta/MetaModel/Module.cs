using System.Collections.Generic;
using DGen.Meta.MetaModel.Types;

namespace DGen.Meta.MetaModel
{
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
        public string FullName => ParentModule != null ? $"{ParentModule.FullName}.{Name}" : Name;
        public string Description { get; set; }
        public List<Module> Modules { get; set; }
        public List<Aggregate> Aggregates { get; set; }
        public List<Entity> Entities { get; set; }
        public List<Value> Values { get; set; }
        public List<DomainEvent> DomainEvents { get; set; }
        public List<Enumeration> Enumerations { get; set; }
        public List<Query> Queries { get; set; }
        public List<ViewModel> ViewModels { get; set; }
    }
}