using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta;

namespace DGen.Generation.Generators.Domain
{
    public class DomainEventCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Domain";

        
        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.DomainEvents;
        }

        public NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("DomainEvents");
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            
        }
    }
}
