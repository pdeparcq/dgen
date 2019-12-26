using System;
using System.Collections.Generic;
using System.Linq;
using DGen.StarUml;

namespace DGen.Meta
{
    public abstract class MetaGeneratorBase<T> : IMetaGenerator where T : BaseType, new()
    {
        public abstract string StereoType { get; }

        public Type GeneratedType => typeof(T);

        public virtual IEnumerable<Element> QueryElements(Element parent)
        {
            var elements = new List<Element>();
            if (parent.OwnedElements != null && parent.OwnedElements.Any())
            {
                foreach (var element in parent.OwnedElements.Where(e => e.Type == ElementType.UMLClass))
                {
                    elements.AddRange(QueryElements(element));
                }
                elements.AddRange(parent.OwnedElements.Where(e => e.Type == ElementType.UMLClass && e.Stereotype?.ToLower() == StereoType.ToLower()));
            }
            return elements;
        }

        public virtual T GenerateType(Element e, Module module, ITypeRegistry registry)
        {
            var t = new T
            {
                Module = module,
                Name = e.Name,
                Description = e.Documentation
            };

            //register type
            registry.Register(e, t);

            return t;
        }

        public abstract List<T> GetListFromModule(Module module);

        public virtual void Generate(T type, Element e, ITypeRegistry registry)
        {
            // Generate properties from attributes
            GenerateAttributeProperties(type, e, registry);

            // Generate properties from associations
            GenerateAssociationProperties(type, e, registry);
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

        private void GenerateAssociationProperties(T type, Element element, ITypeRegistry registry)
        {
            if (element.OwnedElements != null && element.OwnedElements.Any())
            {
                foreach (var association in element.OwnedElements.Where(e => e.Type == ElementType.UMLAssociation && e.AssociationEndFrom.Reference.Element == element))
                {
                    var resolved = registry.Resolve(association.AssociationEndTo.Reference.Element);

                    if (ShouldGenerateProperty(resolved, association.Stereotype))
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

        protected IEnumerable<(Element Element, A Type)> GetAssociations<A>(Element element, ITypeRegistry registry, string stereoType = null) where A : BaseType
        {
            return element.OwnedElements?
                .Where(e => e.Type == ElementType.UMLAssociation && (stereoType == null || e.Stereotype?.ToLower() == stereoType) && registry.Resolve(e.AssociationEndTo.Reference.Element) is A)
                .Select(association => (association, registry.Resolve(association.AssociationEndTo.Reference.Element) as A));     
        }

        protected (Element Element, A Type)? GetAssociation<A>(Element element, ITypeRegistry registry, string stereoType = null) where A : BaseType
        {
            var associations = GetAssociations<A>(element, registry, stereoType);
            if (associations != null && associations.Any())
                return associations.First();
            return null;
        }

        protected IEnumerable<(Element Element, D Type)> GetDependencies<D>(Element element, ITypeRegistry registry, string stereoType = null) where D : BaseType
        {
            return element.OwnedElements?
                .Where(e => e.Type == ElementType.UMLDependency && (stereoType == null || e.Stereotype?.ToLower() == stereoType) && registry.Resolve(e.Target.Element) is D)
                .Select(dependency => (dependency, registry.Resolve(dependency.Target.Element) as D));
        }

        protected (Element Element, D Type)? GetDependency<D>(Element element, ITypeRegistry registry, string stereoType = null) where D : BaseType
        {
            var dependencies = GetDependencies<D>(element, registry, stereoType);
            if (dependencies != null && dependencies.Any())
                return dependencies.First();
            return null;
        }

        protected virtual bool ShouldGenerateProperty(BaseType resolved, string stereoType)
        {
            return resolved != null && stereoType == null;
        }

        public void GenerateTypes(Element parent, Module module, ITypeRegistry registry)
        {
            var list = GetListFromModule(module);
            foreach(var element in QueryElements(parent))
            {
                list.Add(GenerateType(element, module, registry));
            }
        }

        public void Generate(BaseType type, Element e, ITypeRegistry registry)
        {
            Generate(type as T, e, registry);
        }
    }
}