using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta;

namespace DGen.Generation.Generators.Domain
{
    public class AggregateCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Domain";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Aggregates;
        }

        public void Visit(Module module, NamespaceModel @namespace)
        {
            
        }

        public void Visit(BaseType type, NamespaceModel @namespace)
        {
            if (type is Aggregate aggregate)
            {
                var @class = @namespace.AddClass($"{aggregate.Name}");

                foreach(var p in aggregate.Properties)
                {
                    @class.AddDomainProperty(p);
                }
            }
        }

        
    }
}
