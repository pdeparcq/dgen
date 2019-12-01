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
                Services = model.OwnedElements.Where(e => e.Type == ElementType.UMLModel).Select(ToModule).ToList()
            };
        }

        private static Module ToModule(Element s)
        {
            return new Module
            {
                Name = s.Name,
                Modules = s.OwnedElements.Where(e => e.Type == ElementType.UMLPackage).Select(ToModule).ToList(),
                Aggregates = s.OwnedElements.Where(e => e.Type == ElementType.UMLClass && e.Stereotype.ToLower() == "aggregate").Select(ToAggregate).ToList()
            };
        }

        private static Aggregate ToAggregate(Element a)
        {
            return new Aggregate
            {
                Name = a.Name
            };
        }
    }
}
