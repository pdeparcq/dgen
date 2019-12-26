using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;

namespace DGen.Generation.Generators.Application
{
    public class ViewModelCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Application";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.ViewModels;
        }

        public NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("ViewModels");
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is ViewModel viewModel && model is ClassModel @class)
            {
                foreach (var property in viewModel.Properties)
                {
                    @class.AddDomainProperty(property, registry);
                }
                if (viewModel.Target != null)
                {   
                    foreach(var property in viewModel.Target.Properties)
                    {
                        @class.AddDomainProperty(property, registry);
                    }
                }
            }
        }
    }
}
