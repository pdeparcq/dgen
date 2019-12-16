using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class DomainEventMetaGenerator : MetaGeneratorBase<DomainEvent>
    {
        public override string StereoType => "domainevent";

        public override void Generate(DomainEvent domainEvent, Element e, ITypeRegistry registry)
        {
            base.Generate(domainEvent, e, registry);

            /*
            if (aggregate.UniqueIdentifier != null)
            {
                domainEvent.Properties.Insert(0, new Property
                {
                    Name = $"{aggregate.Name}{aggregate.UniqueIdentifier.Name}",
                    Type = aggregate.UniqueIdentifier.Type
                });
            }

            switch (e.Stereotype?.ToLower())
            {
                case "create":
                    domainEvent.Type = DomainEventType.Create;
                    break;
                case "delete":
                    domainEvent.Type = DomainEventType.Delete;
                    break;
                default:
                    domainEvent.Type = DomainEventType.Update;
                    break;
            }
            */
        }
    }
}
