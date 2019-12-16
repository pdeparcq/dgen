using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class EntityMetaGenerator<T> : MetaGeneratorBase<T> where T : Entity, new()
    {
        public override string StereoType => "entity";
    }
}
