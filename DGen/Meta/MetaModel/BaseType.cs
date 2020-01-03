using System.Collections.Generic;

namespace DGen.Meta.MetaModel
{
    public abstract class BaseType
    {
        public BaseType()
        {
            Properties = new List<Property>();
        }

        public Module Module { get; set; }
        public string Name { get; set; }
        public string FullName => $"{Module.FullName}.{Name}";
        public string Description { get; set; }
        public List<Property> Properties { get; set; }
    }
}