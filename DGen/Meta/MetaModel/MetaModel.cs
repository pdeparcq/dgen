using System.Collections.Generic;

namespace DGen.Meta.MetaModel
{
    public class MetaModel
    {
        public MetaModel()
        {
            Services = new List<Module>();
        }

        public string Name { get; set; }
        public List<Module> Services { get; set; }
    }
}
