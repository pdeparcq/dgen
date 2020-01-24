using System.Collections.Generic;
using System.Linq;

namespace DGen.Meta.MetaModel
{
    public class Module
    {
        private readonly List<BaseType> _types;

        public Module()
        {
            Modules = new List<Module>();
            _types = new List<BaseType>();
        }

        public Module ParentModule { get; set; }
        public string Name { get; set; }
        public string FullName => ParentModule != null ? $"{ParentModule.FullName}.{Name}" : Name;
        public string Description { get; set; }
        public List<Module> Modules { get; set; }

        public IEnumerable<T> GetTypes<T>() where T : BaseType
        {
            return _types.OfType<T>().Where(t => t.GetType() == typeof(T)).ToList().AsReadOnly();
        }

        public T AddType<T>(T type) where T : BaseType
        {
            if(!_types.Contains(type))
                _types.Add(type);

            return type;
        }
    }
}