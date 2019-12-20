using System.Collections.Generic;

namespace DGen.Meta.Generators
{
    public class ValueMetaGenerator : MetaGeneratorBase<Value>
    {
        public override string StereoType => "value";

        public override List<Value> GetListFromModule(Module module)
        {
            return module.Values;
        }
    }
}
