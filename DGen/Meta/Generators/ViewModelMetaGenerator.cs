﻿using System.Collections.Generic;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
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

        public override void GenerateTypes(Element parent, Module module, ITypeRegistry registry)
        {
            base.GenerateTypes(parent, module, registry);
            GenerateViewModels(module, module.Aggregates);
            GenerateViewModels(module, module.Entities);
            GenerateViewModels(module, module.Values);
        }

        private static void GenerateViewModels(Module module, IEnumerable<BaseType> types)
        {
            foreach (var type in types)
            {
                // Compact version
                module.ViewModels.Add(new ViewModel
                {
                    Module = module,
                    Name = type.Name,
                    Target = type,
                    IsCompact = true
                });
                // Detail version
                module.ViewModels.Add(new ViewModel
                {
                    Module = module,
                    Name = type.Name,
                    Target = type,
                    IsCompact = false
                });
            }
        }

        public override void Generate(ViewModel viewModel, Element element, ITypeRegistry registry)
        {
            base.Generate(viewModel, element, registry);

            var dependency = GetDependency<Aggregate>(element, registry);

            if(dependency != null)
            {
                viewModel.Target = dependency.Value.Type;
            }
        }
    }
}
