using System.Collections.Generic;
using System.IO;
using DGen.Generation.CodeModel;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis;

namespace DGen.Generation.Generators.Domain
{
    public class EntityCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Domain";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Entities;
        }

        public void Visit(Module module, NamespaceModel @namespace)
        {
            
        }

        public void Visit(BaseType type, NamespaceModel @namespace)
        {
            if (type is Entity entity)
            {
                @namespace = @namespace.AddNamespace("Entities");
                var @class = @namespace.AddClass($"{entity.Name}");

                foreach (var p in entity.Properties)
                {
                    @class.AddDomainProperty(p);
                }
            }
        }
    }
}
