using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Domain
{
    public class DomainEventCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Domain";

        
        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.GetTypes<DomainEvent>().Where(e => e.Aggregate != null);
        }

        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Events");
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is DomainEvent domainEvent && model is ClassModel @class)
            {
                @class = @class
                    .WithBaseType(SystemTypes.DomainEvent(@class))
                    .WithAttributes(SystemTypes.Parse("Serializable"));

                // Generate property for aggregate unique id
                if(domainEvent.Aggregate.UniqueIdentifier != null)
                {
                    @class.AddDomainProperty(new Property { 
                        Name = domainEvent.Aggregate.Name,
                        Description = "Unique id of aggregate that throws this event",
                        Type = new MetaType
                        {
                            Type = domainEvent.Aggregate
                        }
                    }, registry);
                }
                    

                // Generate properties
                foreach (var p in domainEvent.Properties)
                {
                    @class.AddDomainProperty(p, registry);
                }

                // Generate constructor
                @class.AddConstructor()
                    .WithPropertyParameters()
                    .WithAttributes(SystemTypes.JsonConstructorAttribute())
                    .WithBody(builder => { builder.AssignPropertiesFromParameters(); });
            }
        }
    }
}
