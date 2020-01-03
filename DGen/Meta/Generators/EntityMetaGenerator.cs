using System.Collections.Generic;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class EntityMetaGenerator : MetaGeneratorBase<Entity>
    {
        public override string StereoType => "entity";

        public override List<Entity> GetListFromModule(Module module)
        {
            return module.Entities;
        }
    }
}
