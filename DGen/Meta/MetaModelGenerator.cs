using System.Collections.Generic;
using System.Linq;
using DGen.Meta.Generators;
using DGen.StarUml;

namespace DGen.Meta
{
    public class MetaModelGenerator : ITypeRegistry
    {
        private readonly AggregateMetaGenerator _aggregateMetaGenerator;
        private readonly EntityMetaGenerator<Entity> _entityMetaGenerator;
        private readonly ValueMetaGenerator _valueMetaGenerator;
        private readonly DomainEventMetaGenerator _domainEventMetaGenerator;
        private readonly EnumerationMetaGenerator _enumerationMetaGenerator;

        private Dictionary<Element, BaseType> _types;

        public MetaModelGenerator()
        {
            _aggregateMetaGenerator = new AggregateMetaGenerator();
            _entityMetaGenerator = new EntityMetaGenerator<Entity>();
            _valueMetaGenerator = new ValueMetaGenerator();
            _domainEventMetaGenerator = new DomainEventMetaGenerator();
            _enumerationMetaGenerator = new EnumerationMetaGenerator();
        }
        
        public MetaModel Generate(Element model)
        {
            _types = new Dictionary<Element, BaseType>();
            
            // Generate types
            var metaModel = new MetaModel
            {
                Services = model.OwnedElements?.Where(e => e.Type == ElementType.UMLModel).Select(ToService).ToList()
            };

            // Second pass when all types are available
            foreach (var kv in _types)
            {
                if(kv.Value is Aggregate aggregate)
                    _aggregateMetaGenerator.Generate(aggregate, kv.Key, this);
                else if(kv.Value is Entity entity)
                    _entityMetaGenerator.Generate(entity, kv.Key, this);
                else if(kv.Value is Value value)
                    _valueMetaGenerator.Generate(value, kv.Key, this);
                else if(kv.Value is DomainEvent domainEvent)
                    _domainEventMetaGenerator.Generate(domainEvent, kv.Key, this);
                else if(kv.Value is Enumeration enumeration)
                    _enumerationMetaGenerator.Generate(enumeration, kv.Key, this);
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
                ParentModule = parent
            };

            generated.Modules = m.OwnedElements?.Where(e => e.Type == ElementType.UMLPackage).Select(e => ToModule<Module>(e, generated)).ToList();

            // First generate all types
            generated.Aggregates = _aggregateMetaGenerator.QueryElements(m).Select(e => _aggregateMetaGenerator.GenerateType(e, generated, this)).ToList();
            generated.Values = _valueMetaGenerator.QueryElements(m).Select(e => _valueMetaGenerator.GenerateType(e, generated, this)).ToList();
            generated.Entities = _entityMetaGenerator.QueryElements(m).Select(e => _entityMetaGenerator.GenerateType(e, generated, this)).ToList();
            generated.DomainEvents = _domainEventMetaGenerator.QueryElements(m).Select(e => _domainEventMetaGenerator.GenerateType(e, generated, this)).ToList();
            generated.Enumerations = _enumerationMetaGenerator.QueryElements(m).Select(e => _enumerationMetaGenerator.GenerateType(e, generated, this)).ToList();

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
