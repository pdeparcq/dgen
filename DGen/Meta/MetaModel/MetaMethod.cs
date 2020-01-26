using System.Collections.Generic;
using System.Linq;

namespace DGen.Meta.MetaModel
{

    public class MetaParameter
    {
        public string Name { get; set; }
        public MetaType Type { get; set; }
        public bool IsCollection { get; set; }
        public bool IsReturn { get; set; }
    }

    public class MetaMethod
    {
        private List<MetaParameter> _parameters;

        public MetaMethod(string name)
        {
            Name = name;
            _parameters = new List<MetaParameter>();
        }

        public string Name { get; set; }
        public MetaParameter Return => _parameters.SingleOrDefault(p => p.IsReturn);
        public IReadOnlyCollection<MetaParameter> Parameters => _parameters.Where(p => !p.IsReturn).ToList().AsReadOnly();
        public bool IsConstructor { get; set; }

        public void AddParameter(MetaParameter parameter)
        {
            _parameters.Add(parameter);
        }
    }
}
