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
        public TypeModel PrepareType(BaseType type, NamespaceModel @namespace)
        {
            @namespace = @namespace.AddNamespace("Entities");
            return @namespace.AddClass($"{type.Name}");
        }
       
        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is Entity entity && model is ClassModel @class)
            {
                foreach (var p in entity.Properties)
                {
                    @class.AddDomainProperty(p);
                }
            }
        }
    }
}
