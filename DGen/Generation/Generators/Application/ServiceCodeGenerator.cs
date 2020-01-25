using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Application
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
            if (type is Service service && model is ClassModel @class)
            {
                var @interface = registry.Resolve(Layer, type, $"I{type.Name}") as InterfaceModel;

                @class = @class.WithImplementedInterfaces(@interface);

                foreach(var method in @interface.Methods)
                {
                    @class.AddMethod(method.Name).WithParameters(method.Parameters.ToArray())
                        .WithReturnType(method.ReturnType);
                }


                var constructorParameters = new List<MethodParameter>();

                // Create property for aggregate repository 
                if (service.AggregateRepository != null)
                {
                    var repositoryType = SystemTypes.Repository(registry.Resolve("Domain", service.AggregateRepository));

                    var property = @class.AddProperty($"{service.AggregateRepository.Name}Repository", repositoryType)
                        .MakeReadOnly();

                    constructorParameters.Add(new MethodParameter(property.Name.ToCamelCase(), repositoryType));
                }

                // Create properties for query repositories
                if (service.QueryRepositories.Any())
                {
                    var databaseContext = registry.Resolve("Infrastructure", service.Module);
                    var databaseProperty = @class.AddProperty($"Database", databaseContext)
                        .MakeReadOnly();

                    constructorParameters.Add(new MethodParameter(databaseProperty.Name.ToCamelCase(), databaseContext));

                    foreach (var aggregate in service.QueryRepositories)
                    {
                        @class.AddProperty($"{aggregate.Name}Query", SystemTypes.Queryable(registry.Resolve("Infrastructure", aggregate)))
                            .WithGetter(builder =>
                            {
                                builder.Return(databaseProperty.AccessProperty($"{aggregate.Name}Set"));
                            })
                            .WithoutSetter();
                    }
                }

                // Create properties for services
                foreach (var s in service.Services)
                {
                    var serviceType = registry.Resolve(Layer, s, $"I{s.Name}") as InterfaceModel;

                    var property = @class.AddProperty(s.Name, serviceType)
                        .MakeReadOnly();

                    constructorParameters.Add(new MethodParameter(property.Name.ToCamelCase(), serviceType));
                }

                // Generate constructor
                @class.AddConstructor()
                    .WithParameters(constructorParameters.ToArray())
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
