using System.Collections.Generic;

namespace DGen.Meta.Generators
{
    public class QueryMetaGenerator : MetaGeneratorBase<Query>
    {
        public override string StereoType => "query";

        public override List<Query> GetListFromModule(Module module)
        {
            return module.Queries;
        }
    }
}
