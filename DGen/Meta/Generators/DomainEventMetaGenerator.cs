using DGen.StarUml;
using DGen.Meta.MetaModel.Types;

namespace DGen.Meta.Generators
{
    public class DomainEventMetaGenerator : MetaGeneratorBase<DomainEvent>
    {
        public override string StereoType => "domainevent";

        public override void Generate(DomainEvent domainEvent, Element element, ITypeRegistry registry)
        {
            base.Generate(domainEvent, element, registry);

            var association = GetDependency<Aggregate>(element, registry);

            if(association != null)
            {
                domainEvent.Aggregate = association.Value.Type;
            }
        }
    }
}
