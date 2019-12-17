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
                e.Type == ElementType.UMLClass && e.Stereotype?.ToLower() == StereoType.ToLower()) ?? new List<Element>();
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
            // Generate properties from attributes
            GenerateAttributeProperties(type, e, registry);

            // Generate properties from associations
            GenerateAssociationProperties(type, e, registry);
        }

        private static void GenerateAssociationProperties(T type, Element element, ITypeRegistry registry)
        {
            if (element.OwnedElements != null && element.OwnedElements.Any())
            {
                foreach (var association in element.OwnedElements.Where(e => e.Type == ElementType.UMLAssociation && e.AssociationEndFrom.Reference.Element == element))
                {
                    var resolved = registry.Resolve(association.AssociationEndTo.Reference.Element);

                    if (resolved is Aggregate aggregate && aggregate.UniqueIdentifier != null)
                    {
                        var property = new Property
                        {
                            IsCollection = association.AssociationEndTo.Multiplicity?.Contains("*") ?? false,
                            Name = $"{association.AssociationEndTo.Name ?? aggregate.Name}",
                            Type = aggregate.UniqueIdentifier.Type
                        };

                        if (!property.IsCollection)
                            property.Name += aggregate.UniqueIdentifier.Name;

                        type.Properties.Add(property);
                    }
                    else if(resolved != null)
                    {
                        type.Properties.Add(new Property
                        {
                            IsCollection = association.AssociationEndTo.Multiplicity?.Contains("*") ?? false,
                            Name = association.AssociationEndTo.Name ?? resolved.Name,
                            Type = new PropertyType
                            {
                                Type = resolved
                            }
                        });
                    }
                }
            }
        }

        private static void GenerateAttributeProperties(T type, Element e, ITypeRegistry registry)
        {
            if (e.Attributes != null && e.Attributes.Any())
            {
                type.Properties = e.Attributes.Where(p => p.Type == ElementType.UMLAttribute).Select(p => new Property
                {
                    IsIdentifier = p.Stereotype?.ToLower() == "id",
                    Name = p.Name,
                    Type = new PropertyType
                    {
                        SystemType = p.AttributeType?.SystemType,
                        Type = registry.Resolve(p.AttributeType?.ReferenceType?.Element)
                    }
                }).ToList();
            }
            else
            {
                type.Properties = new List<Property>();
            }
        }
    }
}