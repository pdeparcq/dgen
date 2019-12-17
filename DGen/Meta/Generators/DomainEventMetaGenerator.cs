using DGen.StarUml;
using System;
using System.Linq;

namespace DGen.Meta.Generators
{
    public class DomainEventMetaGenerator : MetaGeneratorBase<DomainEvent>
    {
        public override string StereoType => "domainevent";

        public override void Generate(DomainEvent domainEvent, Element element, ITypeRegistry registry)
        {
            base.Generate(domainEvent, element, registry);

            var association = element.OwnedElements?.FirstOrDefault(e => e.Type == ElementType.UMLAssociation && registry.Resolve(e.AssociationEndTo.Reference.Element) is Aggregate);

            if(association != null)
            {
                var aggregate = registry.Resolve(association.AssociationEndTo.Reference.Element) as Aggregate;

                domainEvent.Aggregate = aggregate;
                domainEvent.Type = GenerateDomainEventType(association.Stereotype);
                aggregate.DomainEvents.Add(domainEvent);
            }
        }

        private DomainEventType GenerateDomainEventType(string stereotype)
        {
            if (!Enum.TryParse(stereotype, true, out DomainEventType result))
            {
                result = DomainEventType.Update;
            }
            return result;
        }
    }
}
