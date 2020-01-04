using System.Collections.Generic;
using System.Linq;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class CommandMetaGenerator : MetaGeneratorBase<Command>
    {
        public override string StereoType => "command";
        
        public override void Generate(Command command, Element element, ITypeRegistry registry)
        {
            base.Generate(command, element, registry);

            var association = GetAssociation<InputModel>(element, registry);

            if (association != null)
            {
                command.Input = association.Value.Type;
                command.IsCollection = association.Value.Element.AssociationEndTo.Multiplicity?.Contains("*") ?? false;
            }
            else
            {
                var domainEvent = GetDependency<DomainEvent>(element, registry);

                if (domainEvent != null)
                {
                    command.DomainEvent = domainEvent.Value.Type;

                    var aggregate = GetDependency<Aggregate>(element, registry);

                    if (aggregate != null)
                    {
                        command.MethodName = aggregate.Value.Element.Name ?? command.Name;
                    }
                    else
                    {
                        command.MethodName = command.Name;
                    }
                    
                    command.Input = command.Module.GetTypes<InputModel>().FirstOrDefault(i => i.Source == domainEvent.Value.Type);
                    
                    if (command.Input != null)
                    {
                        command.Input.Name = command.Name;
                    }
                }
            }
        }

        protected override bool ShouldGenerateProperty(BaseType resolved, string stereoType)
        {
            return base.ShouldGenerateProperty(resolved, stereoType) && !(resolved is InputModel);
        }
    }
}
