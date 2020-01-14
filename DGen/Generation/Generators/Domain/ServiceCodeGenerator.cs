using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Domain
{
    public class ServiceCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Application";


        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Services");
        }

        public override string GetTypeName(BaseType type)
        {
            return $"{type.Name}Base";
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if(type is Service service && model is ClassModel @class)
            {
                var @interface = registry.Resolve(Layer, type, $"I{type.Name}") as InterfaceModel;

                @class = @class.WithImplementedInterfaces(@interface);

                if(service.AggregateRepository != null)
                {
                    var repositoryType = SystemTypes.Repository(registry.Resolve("Domain", service.AggregateRepository));

                    @class.AddProperty($"{service.AggregateRepository.Name}Repository", repositoryType)
                        .MakeReadOnly();
                }

                foreach(var repository in service.QueryRepositories)
                {
                    var repositoryType = SystemTypes.Queryable(registry.Resolve("Infrastructure", repository));

                    @class.AddProperty($"{repository.Name}Query", repositoryType)
                        .MakeReadOnly();
                }

                foreach (var s in service.Services)
                {
                    var serviceType = registry.Resolve(Layer, s, $"I{s.Name}") as InterfaceModel;

                    @class.AddProperty(s.Name, serviceType)
                        .MakeReadOnly();
                }

                @class.AddConstructor()
                    .WithPropertyParameters()
                    .WithBody(builder =>
                    {
                        builder.AssignPropertiesFromParameters();
                    })
                    .MakeProtected();
            }
        }

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.GetTypes<Service>();
        }
    }
}
