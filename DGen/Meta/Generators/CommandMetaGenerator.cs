using System.Collections.Generic;
using System.Linq;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class CommandMetaGenerator : MetaGeneratorBase<Command>
    {
        public override string StereoType => "command";
        
        public override void Generate(Command command, Element element, ITypeRegistry registry)
        {
            base.Generate(command, element, registry);

            command.Service = GetDependency<Service>(element, registry)?.Type;
        }

        protected override bool ShouldGenerateProperty(BaseType resolved, string stereoType)
        {
            return base.ShouldGenerateProperty(resolved, stereoType) && !(resolved is InputModel);
        }
    }
}
