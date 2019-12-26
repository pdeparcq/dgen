using System.Collections.Generic;
using System.Linq;
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

        public TypeModel PrepareType(BaseType type, NamespaceModel @namespace)
        {
            @namespace = @namespace.AddNamespace("Entities");
            return @namespace.AddClass($"{type.Name}");
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            if (GetTypes(module).Any())
                @namespace.AddClass($"{module.Name}DbContext");
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            
        }
    }
}
