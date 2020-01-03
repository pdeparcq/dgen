using System.Collections.Generic;

namespace DGen.Meta.MetaModel
{
    public class MetaModel
    {
        public MetaModel()
        {
            Services = new List<Service>();
        }

        public string Name { get; set; }
        public List<Service> Services { get; set; }
    }
}
