using System.Collections.Generic;

namespace DGen.Test.Meta
{
    public class MetaModel
    {
        public List<Module> Services { get; set; }
    }

    public class Module
    {
        public string Name { get; set; }
        public List<Module> Modules { get; set; }
        public List<Aggregate> Aggregates { get; set; }
    }

    public class Aggregate
    {
        public string Name { get; set; }
    }
}
