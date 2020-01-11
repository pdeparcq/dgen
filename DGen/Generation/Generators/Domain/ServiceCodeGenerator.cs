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
        public override string Layer => "Domain";


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

                foreach(var repository in service.Repositories)
                {
                    var repositoryType = SystemTypes.Repository(registry.Resolve(Layer, repository));

                    @class.AddProperty($"{repository.Name}Repository", repositoryType)
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
