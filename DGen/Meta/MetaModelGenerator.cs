using System.Collections.Generic;
using System.Linq;
using DGen.StarUml;

namespace DGen.Meta
{
    public class MetaModelGenerator
    {
        private Dictionary<string, BaseType> _types;

        public MetaModel Generate(Element model)
        {
            _types = new Dictionary<string, BaseType>();
            return new MetaModel
            {
                Services = model.OwnedElements?.Where(e => e.Type == ElementType.UMLModel).Select(ToService).ToList()
            };
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
            generated.Values = m.OwnedElements?.Where(e => e.Type == ElementType.UMLClass && e.Stereotype?.ToLower() == "value").Select(v => ToValue(v, generated)).ToList();
            generated.Entities = m.OwnedElements?.Where(e => e.Type == ElementType.UMLClass && e.Stereotype?.ToLower() == "entity").Select(e => ToEntity<Entity>(e, generated)).ToList();
            generated.Aggregates = m.OwnedElements?.Where(e => e.Type == ElementType.UMLClass && e.Stereotype?.ToLower() == "aggregate").Select(a => ToAggregate(a, generated)).ToList();

            return generated;
        }

        private Value ToValue(Element v, Module m)
        {
            return ToBaseType<Value>(v, m);
        }

        private T ToEntity<T>(Element e, Module m) where T : Entity, new()
        {
            return ToBaseType<T>(e, m);
        }

        private Aggregate ToAggregate(Element a, Module m)
        {
            var aggregate = ToEntity<Aggregate>(a, m);
            aggregate.DomainEvents = a.OwnedElements?
                    .Where(e => e.Type == ElementType.UMLDependency && e.Source == a && e.Target?.Stereotype?.ToLower() == "domainevent")
                    .Select(de => ToDomainEvent(de, aggregate)).ToList() ?? new List<DomainEvent>();
            return aggregate;
        }

        private DomainEvent ToDomainEvent(Element de, Aggregate aggregate)
        {
            var domainEvent = ToBaseType<DomainEvent>(de.Target, aggregate.Module);
            
            if(aggregate.UniqueIdentifier != null)
            {
                domainEvent.Properties.Insert(0, new Property
                {
                    Name = $"{aggregate.Name}{aggregate.UniqueIdentifier.Name}",
                    Type = aggregate.UniqueIdentifier.Type
                });
            }   
            switch (de.Stereotype?.ToLower())
            {
                case "create":
                    domainEvent.Type = DomainEventType.Create;
                    break;
                case "delete":
                    domainEvent.Type = DomainEventType.Delete;
                    break;
                default:
                    domainEvent.Type = DomainEventType.Update;
                    break;
            }
            return domainEvent;
        }

        private T ToBaseType<T>(Element e, Module m) where T : BaseType, new()
        {
            var t = new T()
            {
                Module = m,
                Name = e.Name,
            };

            // Store type in registry
            _types[e.FullName] = t;

            // Generate properties
            if(e.Attributes != null && e.Attributes.Any())
            {
                t.Properties = e.Attributes.Where(p => p.Type == ElementType.UMLAttribute).Select(p => new Property
                {
                    IsIdentifier = p.Stereotype?.ToLower() == "id",
                    Name = p.Name,
                    Type = new PropertyType
                    {
                        SystemType = p.AttributeType?.SystemType,
                        Type = GetType(p.AttributeType?.ReferenceType)
                    }
                }).ToList();
            }
            else
            {
                t.Properties = new List<Property>();
            }

            // Generate properties from associations
            if (e.OwnedElements != null && e.OwnedElements.Any())
            {
                foreach (var association in e.OwnedElements.Where(oe => oe.Type == ElementType.UMLAssociation && oe.AssociationEndFrom.Reference == e))
                {
                    if (GetType(association.AssociationEndTo.Reference) is Aggregate aggregate)
                    {
                        if (aggregate.UniqueIdentifier != null)
                        {
                            t.Properties.Add(new Property
                            {
                                IsIdentifier = association.AssociationEndTo.Stereotype?.ToLower() == "id",
                                Name = $"{association.AssociationEndTo.Name ?? aggregate.Name}{aggregate.UniqueIdentifier.Name}",
                                Type = aggregate.UniqueIdentifier.Type
                            });
                        }
                    }
                }
            }
            
            return t;
        }

        private BaseType GetType(Element element)
        {
            if (element != null)
            {
                if (_types.ContainsKey(element.FullName))
                {
                    return _types[element.FullName];
                }
            }
            return null;
        }
    }
}
