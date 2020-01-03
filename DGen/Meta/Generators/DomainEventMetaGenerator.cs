using DGen.StarUml;
using System;
using System.Collections.Generic;
using System.Linq;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Meta.Generators
{
    public class DomainEventMetaGenerator : MetaGeneratorBase<DomainEvent>
    {
        public override string StereoType => "domainevent";

        protected override bool ShouldGenerateProperty(BaseType resolved, string stereoType)
        {
            return resolved != null;
        }

        public override void Generate(DomainEvent domainEvent, Element element, ITypeRegistry registry)
        {
            base.Generate(domainEvent, element, registry);

            var association = GetAssociation<Aggregate>(element, registry);

            if(association != null)
            {
                domainEvent.Aggregate = association.Value.Type;
                domainEvent.Type = GenerateDomainEventType(association.Value.Element.Stereotype);
                domainEvent.Aggregate.DomainEvents.Add(domainEvent);
            }
        }

        public override List<DomainEvent> GetListFromModule(Module module)
        {
            return module.DomainEvents;
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
