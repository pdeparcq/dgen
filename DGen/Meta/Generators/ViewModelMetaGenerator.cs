using System.Collections.Generic;
using System.Linq;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class ViewModelMetaGenerator : MetaGeneratorBase<ViewModel>
    {
        public override string StereoType => "viewmodel";

        public override void GenerateTypes(Element parent, Module module, ITypeRegistry registry)
        {
            base.GenerateTypes(parent, module, registry);
            GenerateViewModels(module, module.GetTypes<Aggregate>());
            GenerateViewModels(module, module.GetTypes<Entity>());
            GenerateViewModels(module, module.GetTypes<Value>());
        }

        private static void GenerateViewModels(Module module, IEnumerable<BaseType> types)
        {
            foreach (var type in types)
            {
                // Compact version
                module.AddType(new ViewModel
                {
                    Module = module,
                    Name = type.Name,
                    Description = $"ViewModel for type: {type.FullName}",
                    Target = type,
                    IsCompact = true
                });
                // Detail version
                module.AddType(new ViewModel
                {
                    Module = module,
                    Name = type.Name,
                    Description = $"Compact ViewModel for type: {type.FullName}",
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
