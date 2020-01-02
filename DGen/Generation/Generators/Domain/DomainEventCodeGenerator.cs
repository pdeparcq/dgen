using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
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
            if (type is DomainEvent domainEvent && model is ClassModel @class)
            {
                @class = @class
                    .WithBaseType(SystemTypes.DomainEvent(@class))
                    .WithAttributes(SystemTypes.Parse("Serializable"));

                // Generate properties
                foreach (var p in domainEvent.Properties)
                {
                    @class.AddDomainProperty(p, registry);
                }

                // Generate constructor
                @class.AddConstructor()
                    .WithPropertyParameters()
                    .WithAttributes(SystemTypes.JsonConstructorAttribute())
                    .WithBody(builder => { builder.AssignProperties(); });
            }
        }
    }
}
