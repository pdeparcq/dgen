using System.Collections.Generic;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class AggregateMetaGenerator : EntityMetaGenerator<Aggregate>
    {
        public override string StereoType => "aggregate";

        public override Aggregate GenerateType(Element e, Module module, ITypeRegistry registry)
        {
            var aggregate =  base.GenerateType(e, module, registry);
            aggregate.DomainEvents = new List<DomainEvent>();
            return aggregate;
        }
    }
}
