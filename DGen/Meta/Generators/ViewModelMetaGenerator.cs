using System.Collections.Generic;

namespace DGen.Meta.Generators
{
    public class ViewModelMetaGenerator : MetaGeneratorBase<ViewModel>
    {
        public override string StereoType => "viewmodel";

        public override List<ViewModel> GetListFromModule(Module module)
        {
            return module.ViewModels;
        }
    }
}
