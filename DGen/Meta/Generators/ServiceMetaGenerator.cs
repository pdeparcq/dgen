using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class ServiceMetaGenerator : MetaGeneratorBase<Service>
    {
        public override string StereoType => "service";

        public override void Generate(Service service, Element element, ITypeRegistry registry)
        {
            base.Generate(service, element, registry);

            var repositories = base.GetDependencies<Aggregate>(element, registry);

            foreach(var repository in repositories)
            {
                if(!service.Repositories.Contains(repository.Type))
                    service.Repositories.Add(repository.Type);
            }
        }
    }
}
