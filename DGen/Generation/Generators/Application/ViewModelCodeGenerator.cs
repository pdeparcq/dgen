using System.Collections.Generic;
using System.IO;
using DGen.Generation.CodeModel;
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

        public TypeModel PrepareType(BaseType type, NamespaceModel @namespace)
        {
            @namespace = @namespace.AddNamespace("ViewModels");
            return @namespace.AddClass($"{type.Name}ViewModel");
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            
        }
    }
}
