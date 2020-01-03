using System.Collections.Generic;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Meta.Generators
{
    public class ValueMetaGenerator : MetaGeneratorBase<Value>
    {
        public override string StereoType => "value";
    }
}
