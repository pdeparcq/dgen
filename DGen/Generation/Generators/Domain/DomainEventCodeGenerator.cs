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

        public TypeModel PrepareType(BaseType type, NamespaceModel @namespace)
        {
            @namespace = @namespace.AddNamespace("DomainEvents");
            return @namespace.AddClass($"{type.Name}");
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            
        }
    }
}
