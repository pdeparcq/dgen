using DGen.Test.StarUml;
using System.Linq;

namespace DGen.Test.Meta
{
    public class MetaModelGenerator
    {
        public MetaModel Generate(Element model)
        {
            return new MetaModel
            {
                Services = model.OwnedElements.Where(e => e.Type == ElementType.UMLModel).Select(ToService).ToList()
            };
        }

        private static Service ToService(Element s)
        {
            return ToModule<Service>(s);
        }

        private static T ToModule<T>(Element m) where T : Module, new()
        {
            return new T()
            {
                Name = m.Name,
                Modules = m.OwnedElements.Where(e => e.Type == ElementType.UMLPackage).Select(ToModule<Module>).ToList(),
                Aggregates = m.OwnedElements.Where(e => e.Type == ElementType.UMLClass && e.Stereotype.ToLower() == "aggregate").Select(ToAggregate).ToList(),
                Entities = m.OwnedElements.Where(e => e.Type == ElementType.UMLClass && e.Stereotype.ToLower() == "entity").Select(ToEntity<Entity>).ToList(),
                Values = m.OwnedElements.Where(e => e.Type == ElementType.UMLClass && e.Stereotype.ToLower() == "value").Select(ToValue).ToList()
            };
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
                    Type = p.AttributeType
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
