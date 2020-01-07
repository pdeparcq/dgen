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

        protected override ElementType ElementType => ElementType.UMLEnumeration;

        public override void Generate(Enumeration type, Element element, ITypeRegistry registry)
        {
            type.Literals = element.Literals?.Select(l => l.Name).ToList() ?? new List<string>();
        }
    }
}
