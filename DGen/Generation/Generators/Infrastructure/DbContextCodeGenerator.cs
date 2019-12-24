using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta;

namespace DGen.Generation.Generators.Infrastructure
{
    public class DbContextCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Infrastructure";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Aggregates;
        }

        public void Visit(Module module, NamespaceModel @namespace)
        {
            @namespace.AddClass($"{module.Name}DbContext");
        }

        public void Visit(BaseType type, NamespaceModel @namespace)
        {
            if (type is Aggregate aggregate)
            {
                @namespace.AddClass($"{aggregate.Name}");
            }
        }
    }
}
