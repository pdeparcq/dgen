using DGen.Meta.MetaModel.Types;

namespace DGen.Meta.Generators
{
    public class AggregateMetaGenerator : MetaGeneratorBase<Aggregate>
    {
        public override string StereoType => "aggregate";
    }
}
