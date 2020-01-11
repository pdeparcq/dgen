using System;
using System.Collections.Generic;
using System.Linq;
using DGen.Meta.Generators;
using DGen.Meta.MetaModel;
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
            AddMetaGenerator(new CommandMetaGenerator());
            AddMetaGenerator(new InputModelMetaGenerator());
            AddMetaGenerator(new ServiceMetaGenerator());
        }

        private void AddMetaGenerator(IMetaGenerator metaGenerator)
        {
            _metaGenerators[metaGenerator.GeneratedType] = metaGenerator;
        }

        public MetaModel.MetaModel Generate(Element model)
        {
            _types = new Dictionary<Element, BaseType>();

            // Generate types
            var metaModel = new MetaModel.MetaModel
            {
                Name = model.Name,
                Services = model.OwnedElements?.Where(e => e.Type == ElementType.UMLModel).Select(e => ToModule(e, null)).ToList() ?? new List<Module>()
            };

            // Second pass when all types are available
            foreach (var kv in _types)
            {
                _metaGenerators[kv.Value.GetType()].Generate(kv.Value, kv.Key, this);
            }

            // Clean up services
            foreach (var service in metaModel.Services)
            {
                CleanUp(service);
            }

            return metaModel;
        }

        private void CleanUp(Module module)
        {
            foreach (var metaGenerator in _metaGenerators.Values)
            {
                metaGenerator.CleanUp(module);
            }
            module.Modules.ForEach(CleanUp);
        }

        private Module ToModule(Element m, Module parent = null)
        {
            var generated = new Module()
            {
                Name = m.Name,
                ParentModule = parent,
                Description = m.Documentation
            };

            generated.Modules = m.OwnedElements?.Where(e => e.Type == ElementType.UMLPackage).Select(e => ToModule(e, generated)).ToList() ?? new List<Module>();

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
