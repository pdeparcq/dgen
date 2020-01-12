using System;
using System.Collections.Generic;
using System.Linq;
using DGen.Meta.MetaModel;
using DGen.StarUml;

namespace DGen.Meta
{
    public abstract class MetaGeneratorBase<T> : IMetaGenerator where T : BaseType, new()
    {
        public abstract string StereoType { get; }

        protected virtual ElementType ElementType => ElementType.UMLClass;

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
                elements.AddRange(parent.OwnedElements.Where(e => e.Type == ElementType && e.Stereotype?.ToLower() == StereoType.ToLower()));
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

        public virtual void Generate(T type, Element e, ITypeRegistry registry)
        {
            // Generate properties from associations
            GenerateAssociationProperties(type, e, registry);

            // Generate properties from attributes
            GenerateAttributeProperties(type, e, registry);

            // Generate methods from operations
            GenerateOperationMethods(type, e, registry);
        }

        private void GenerateOperationMethods(T type, Element element, ITypeRegistry registry)
        {
            if(element.Operations != null && element.Operations.Any())
            {
                type.Methods.AddRange(element.Operations.Where(o => o.Type == ElementType.UMLOperation).Select(o =>
                {
                    var method = new MetaMethod(o.Name);
                    o.Parameters?.ForEach(p => method.AddParameter(new MetaParameter
                    {
                        Name = p.Name,
                        Type = ToMetaType(registry, p.MemberType),
                        IsCollection = p.Multiplicity?.Contains("*") ?? false,
                        IsReturn = p.ParameterDirection != null ? p.ParameterDirection == "return" : false
                    }));
                    return method;
                }));
            }
        }

        private static void GenerateAttributeProperties(T type, Element e, ITypeRegistry registry)
        {
            if (e.Attributes != null && e.Attributes.Any())
            {
                type.Properties.AddRange(e.Attributes.Where(p => p.Type == ElementType.UMLAttribute).Select(p => new Property
                {
                    IsIdentifier = p.Stereotype?.ToLower() == "id",
                    IsCollection = p.Multiplicity?.Contains("*") ?? false,
                    Name = p.Name,
                    Description = p.Documentation,
                    Type = ToMetaType(registry, p.MemberType)
                }));
            }
        }

        private static MetaType ToMetaType(ITypeRegistry registry, MemberType type)
        {
            return new MetaType
            {
                SystemType = type?.SystemType,
                Type = registry.Resolve(type?.ReferenceType?.Element)
            };
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
                            Description = association.Documentation,
                            Type = new MetaType
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

        public virtual void GenerateTypes(Element parent, Module module, ITypeRegistry registry)
        {
            foreach(var element in QueryElements(parent))
            {
                module.AddType(GenerateType(element, module, registry));
            }
        }

        public void Generate(BaseType type, Element e, ITypeRegistry registry)
        {
            Generate(type as T, e, registry);
        }

        public virtual void CleanUp(Module module)
        {
            
        }
    }
}