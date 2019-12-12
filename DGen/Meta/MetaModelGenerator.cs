using System.Linq;
using DGen.StarUml;

namespace DGen.Meta
{
    public class MetaModelGenerator
    {
        public MetaModel Generate(Element model)
        {
            return new MetaModel
            {
                Services = model.OwnedElements?.Where(e => e.Type == ElementType.UMLModel).Select(ToService).ToList()
            };
        }

        private static Service ToService(Element s)
        {
            return ToModule<Service>(s);
        }

        private static T ToModule<T>(Element m, Module parent = null) where T : Module, new()
        {
            var generated = new T()
            {
                Name = m.Name,
                ParentModule = parent
            };

            generated.Modules = m.OwnedElements?.Where(e => e.Type == ElementType.UMLPackage).Select(e => ToModule<Module>(e, generated)).ToList();
            generated.Aggregates = m.OwnedElements?.Where(e => e.Type == ElementType.UMLClass && e.Stereotype?.ToLower() == "aggregate").Select(ToAggregate).ToList();
            generated.Entities = m.OwnedElements?.Where(e => e.Type == ElementType.UMLClass && e.Stereotype?.ToLower() == "entity").Select(ToEntity<Entity>).ToList();
            generated.Values = m.OwnedElements?.Where(e => e.Type == ElementType.UMLClass && e.Stereotype?.ToLower() == "value").Select(ToValue).ToList();

            return generated;
        }

        private static Aggregate ToAggregate(Element a)
        {
            return ToEntity<Aggregate>(a);
        }

        private static T ToEntity<T>(Element e) where T : Entity, new()
        {
            return new T()
            {
                Name = e.Name,
                Properties = e.Attributes?.Where(p => p.Type == ElementType.UMLAttribute).Select(p => new Property
                {
                    Name = p.Name,
                    Type = p.AttributeSimpleType ?? p.AttributeElementType?.Name
                }).ToList()
            };
        }

        public static Value ToValue(Element v)
        {
            return new Value
            {
                Name = v.Name
            };
        }
    }
}
