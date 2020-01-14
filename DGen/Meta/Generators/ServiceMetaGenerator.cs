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

            // Aggregate repository
            var aggregate = base.GetDependency<Aggregate>(element, registry);

            if (aggregate != null)
                service.AggregateRepository = aggregate.Value.Type;

            // Query repositories
            var repositories = base.GetDependencies<Aggregate>(element, registry, "query");

            foreach(var repository in repositories)
            {
                if(!service.QueryRepositories.Contains(repository.Type))
                    service.QueryRepositories.Add(repository.Type);
            }

            // Services
            var services = base.GetDependencies<Service>(element, registry);

            foreach(var s in services)
            {
                if (!service.Services.Contains(s.Type))
                    service.Services.Add(s.Type);
            }
        }
    }
}
