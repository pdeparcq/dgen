using System.Collections.Generic;
using System.Linq;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class QueryMetaGenerator : MetaGeneratorBase<Query>
    {
        public override string StereoType => "query";

        public override void Generate(Query query, Element element, ITypeRegistry registry)
        {
            base.Generate(query, element, registry);

            var association = GetAssociation<ViewModel>(element, registry);

            if(association != null)
            {
                query.Result = association.Value.Type;
                query.IsCollection = association.Value.Element.AssociationEndTo.Multiplicity?.Contains("*") ?? false;
                query.Result.IsCompact = query.IsCollection;
            }
            else
            {
                var aggregateAssociation = GetAssociation<Aggregate>(element, registry, "result");

                if(aggregateAssociation != null)
                {
                    var aggregate = aggregateAssociation.Value.Type;
                    query.IsCollection = aggregateAssociation.Value.Element.AssociationEndTo.Multiplicity?.Contains("*") ?? false;
                    query.Result = query.Module.GetTypes<ViewModel>().FirstOrDefault(vm => vm.Target == aggregate && vm.IsCompact == query.IsCollection);          
                }
            }
        }

        protected override bool ShouldGenerateProperty(BaseType resolved, string stereoType)
        {
            return base.ShouldGenerateProperty(resolved, stereoType) && !(resolved is ViewModel);
        }
    }
}
