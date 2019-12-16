using System.Collections.Generic;
using System.Linq;
using DGen.StarUml;

namespace DGen.Meta
{
    public abstract class MetaGeneratorBase<T> where T : BaseType, new()
    {
        public abstract string StereoType { get; }

        public virtual IEnumerable<Element> QueryElements(Element parent)
        {
            return parent.OwnedElements?.Where(e =>
                e.Type == ElementType.UMLClass && e.Stereotype?.ToLower() == StereoType.ToLower());
        }

        public virtual T GenerateType(Element e, Module module, ITypeRegistry registry)
        {
            var t =  new T
            {
                Module = module,
                Name = e.Name
            };

            //register type
            registry.Register(e, t);

            return t;
        }

        public virtual void Generate(T type, Element e, ITypeRegistry registry)
        {
            // Generate properties
            if (e.Attributes != null && e.Attributes.Any())
            {
                type.Properties = e.Attributes.Where(p => p.Type == ElementType.UMLAttribute).Select(p => new Property
                {
                    IsIdentifier = p.Stereotype?.ToLower() == "id",
                    Name = p.Name,
                    Type = new PropertyType
                    {
                        SystemType = p.AttributeType?.SystemType,
                        Type = registry.Resolve(p.AttributeType?.ReferenceType)
                    }
                }).ToList();
            }
            else
            {
                type.Properties = new List<Property>();
            }

            // Generate properties from associations
            if (e.OwnedElements != null && e.OwnedElements.Any())
            {
                foreach (var association in e.OwnedElements.Where(oe => oe.Type == ElementType.UMLAssociation && oe.AssociationEndFrom.Reference == e))
                {
                    if (registry.Resolve(association.AssociationEndTo.Reference) is Aggregate aggregate)
                    {
                        if (aggregate.UniqueIdentifier != null)
                        {
                            type.Properties.Add(new Property
                            {
                                IsIdentifier = association.AssociationEndTo.Stereotype?.ToLower() == "id",
                                Name = $"{association.AssociationEndTo.Name ?? aggregate.Name}{aggregate.UniqueIdentifier.Name}",
                                Type = aggregate.UniqueIdentifier.Type
                            });
                        }
                    }
                }
            }
        }
    }
}