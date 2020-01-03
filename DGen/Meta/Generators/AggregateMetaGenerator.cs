using System.Collections.Generic;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class AggregateMetaGenerator : MetaGeneratorBase<Aggregate>
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
