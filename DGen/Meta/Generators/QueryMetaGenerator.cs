using System.Collections.Generic;
using System.Linq;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class QueryMetaGenerator : MetaGeneratorBase<Query>
    {
        public override string StereoType => "query";

        public override List<Query> GetListFromModule(Module module)
        {
            return module.Queries;
        }

        public override void Generate(Query query, Element element, ITypeRegistry registry)
        {
            base.Generate(query, element, registry);

            var association = GetAssociation<ViewModel>(element, registry);
            if(association != null)
            {
                query.Result = association.Value.Type;
                query.IsCollection = association.Value.Element.AssociationEndTo.Multiplicity?.Contains("*") ?? false;
            }    
        }

        protected override bool ShouldGenerateProperty(BaseType resolved)
        {
            return resolved != null && !(resolved is ViewModel);
        }
    }
}
