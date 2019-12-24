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

        public void Visit(Module module, NamespaceModel @namespace)
        {
            
        }

        public void Visit(BaseType type, NamespaceModel @namespace)
        {
            if (type is DomainEvent de)
            {
                @namespace.AddClass($"{de.Name}");
            }
        }
    }
}
