using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Application
{
    public class InputModelCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Application";

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.InputModels;
        }

        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("InputModels");
        }

        public override string GetTypeName(BaseType type)
        {
            return $"{type.Name}InputModel";
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is InputModel inputModel && model is ClassModel @class)
            {
                GenerateInputModel(registry, inputModel, @class);
            }
        }

        private void GenerateInputModel(ITypeModelRegistry registry, InputModel inputModel, ClassModel @class)
        {
            foreach (var property in inputModel.Properties)
            {
                @class.AddDomainProperty(property.Denormalized(), registry);
            }
            if (inputModel.Source != null)
            {
                foreach (var property in inputModel.Source.Properties)
                {
                    @class.AddDomainProperty(property.Denormalized(), registry);
                }
            }
        }
    }
}
