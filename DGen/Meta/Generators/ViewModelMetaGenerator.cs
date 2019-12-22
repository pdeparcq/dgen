using System.Collections.Generic;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class ViewModelMetaGenerator : MetaGeneratorBase<ViewModel>
    {
        public override string StereoType => "viewmodel";

        public override List<ViewModel> GetListFromModule(Module module)
        {
            return module.ViewModels;
        }

        public override void Generate(ViewModel viewModel, Element element, ITypeRegistry registry)
        {
            base.Generate(viewModel, element, registry);

            var dependency = GetDependency<Aggregate>(element, registry);

            if(dependency != null)
            {
                viewModel.Target = dependency.Value.Type;
                viewModel.IsCompact = dependency.Value.Element.Stereotype?.ToLower() == "compact";
            }
        }
    }
}
