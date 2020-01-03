using System.Collections.Generic;
using System.Linq;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class InputModelMetaGenerator : MetaGeneratorBase<InputModel>
    {
        public override string StereoType => "inputmodel";

        public override List<InputModel> GetListFromModule(Module module)
        {
            return module.InputModels;
        }

        public override void GenerateTypes(Element parent, Module module, ITypeRegistry registry)
        {
            base.GenerateTypes(parent, module, registry);

            foreach (var domainEvent in module.DomainEvents)
            {
                module.InputModels.Add(new InputModel
                {
                    Module = module,
                    Name = domainEvent.Name,
                    Description = $"Input model for domain event: {domainEvent.FullName}",
                    Source = domainEvent
                });
            }
        }

        public override void Generate(InputModel inputModel, Element element, ITypeRegistry registry)
        {
            base.Generate(inputModel, element, registry);

            var dependency = GetDependency<DomainEvent>(element, registry);

            if (dependency != null)
            {
                inputModel.Source = dependency.Value.Type;
            }
        }

        public override void CleanUp(Module module)
        {
            module.InputModels.RemoveAll(im => module.Commands.All(c => c.Input != im));
        }
    }
}
