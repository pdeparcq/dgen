using System.Collections.Generic;
using System.Linq;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class EnumerationMetaGenerator : MetaGeneratorBase<Enumeration>
    {
        public override string StereoType => "enumeration";

        public override IEnumerable<Element> QueryElements(Element parent)
        {
            return parent.OwnedElements?.Where(e => e.Type == ElementType.UMLEnumeration) ?? new List<Element>();
        }

        public override void Generate(Enumeration type, Element element, ITypeRegistry registry)
        {
            type.Literals = element.Literals?.Select(l => l.Name).ToList() ?? new List<string>();
        }

        public override List<Enumeration> GetListFromModule(Module module)
        {
            return module.Enumerations;
        }
    }
}
