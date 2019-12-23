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
            else
            {
                var aggregateAssociation = GetAssociation<Aggregate>(element, registry, "result");

                if(aggregateAssociation != null)
                {
                    var aggregate = aggregateAssociation.Value.Type;
                    var viewModel = query.Module.ViewModels.FirstOrDefault(vm => vm.Target == aggregate);

                    // Create viewmodel and add it to module if it doesn't exist yet
                    if (viewModel == null)
                    {
                        viewModel = new ViewModel
                        {
                            Module = query.Module,
                            Name = aggregate.Name,
                            Target = aggregate
                        };
                        query.Module.ViewModels.Add(viewModel);
                    }

                    query.Result = viewModel;
                    query.IsCollection = aggregateAssociation.Value.Element.AssociationEndTo.Multiplicity?.Contains("*") ?? false;
                }
            }
        }

        protected override bool ShouldGenerateProperty(BaseType resolved, string stereoType)
        {
            return base.ShouldGenerateProperty(resolved, stereoType) && !(resolved is ViewModel);
        }
    }
}
