using System.Collections.Generic;
using System.Linq;
using DGen.Meta.Generators;
using DGen.StarUml;

namespace DGen.Meta
{
    public class MetaModelGenerator : ITypeRegistry
    {

        private readonly Dictionary<System.Type, IMetaGenerator> _metaGenerators;
        private Dictionary<Element, BaseType> _types;

        public MetaModelGenerator()
        {
            _metaGenerators = new Dictionary<System.Type, IMetaGenerator>();

            AddMetaGenerator(new DomainEventMetaGenerator());
            AddMetaGenerator(new AggregateMetaGenerator());
            AddMetaGenerator(new EntityMetaGenerator());
            AddMetaGenerator(new EnumerationMetaGenerator());
            AddMetaGenerator(new QueryMetaGenerator());
            AddMetaGenerator(new ValueMetaGenerator());
            AddMetaGenerator(new ViewModelMetaGenerator());
        }

        private void AddMetaGenerator(IMetaGenerator metaGenerator)
        {
            _metaGenerators[metaGenerator.GeneratedType] = metaGenerator;
        }

        public MetaModel Generate(Element model)
        {
            _types = new Dictionary<Element, BaseType>();
            
            // Generate types
            var metaModel = new MetaModel
            {
                Name = model.Name,
                Services = model.OwnedElements?.Where(e => e.Type == ElementType.UMLModel).Select(ToService).ToList()
            };

            // Second pass when all types are available
            foreach (var kv in _types)
            {
                _metaGenerators[kv.Value.GetType()].Generate(kv.Value, kv.Key, this);
            }

            return metaModel;
        }

        private Service ToService(Element s)
        {
            return ToModule<Service>(s);
        }

        private T ToModule<T>(Element m, Module parent = null) where T : Module, new()
        {
            var generated = new T()
            {
                Name = m.Name,
                ParentModule = parent,
                Description = m.Documentation
            };

            generated.Modules = m.OwnedElements?.Where(e => e.Type == ElementType.UMLPackage).Select(e => ToModule<Module>(e, generated)).ToList();

            foreach(var metaGenerator in _metaGenerators.Values)
            {
                metaGenerator.GenerateTypes(m, generated, this);
            }

            return generated;
        }

        public BaseType Resolve(Element e)
        {
            if (e != null)
            {
                if (_types.ContainsKey(e))
                {
                    return _types[e];
                }
            }
            return null;
        }

        public void Register(Element e, BaseType type)
        {
            _types[e] = type;
        }
    }
}
