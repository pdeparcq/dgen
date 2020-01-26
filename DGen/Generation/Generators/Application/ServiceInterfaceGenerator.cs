using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using System.Collections.Generic;
using System.Linq;

namespace DGen.Generation.Generators.Application
{
    public class ServiceInterfaceGenerator : LayerCodeGenerator
    {
        public override string Layer => "Application";

        public override string GetTypeName(BaseType type)
        {
            return $"I{type.Name}";
        }

        public override TypeModel PrepareType(NamespaceModel @namespace, BaseType type)
        {
            return GetNamespace(@namespace).AddInterface(GetTypeName(type));
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is Service service && model is InterfaceModel @interface)
            {
                foreach (var method in service.Methods)
                {
                    var serviceMethod = @interface.AddMethod(method.Name)
                        .WithParameters(method.Parameters.Select(p => GenerateMethodParameter(registry, p)).ToArray());

                    serviceMethod = serviceMethod.WithReturnType(SystemTypes.Task(GenerateReturnType(registry, method)));
                }
            }
        }

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.GetTypes<Service>();
        }

        private static MethodParameter GenerateMethodParameter(ITypeModelRegistry registry, MetaParameter p)
        {
            return new MethodParameter(p.GetDomainName(), p.GetDomainType(registry));
        }

        private static TypeModel GenerateReturnType(ITypeModelRegistry registry, MetaMethod method)
        {
            TypeModel returnType = null;

            if (method.Return != null)
            {
                if (method.Return.Type.Type is Aggregate && !method.Return.IsCollection)
                {
                    returnType = SystemTypes.Enumerable(SystemTypes.DomainEvent());
                }
                else
                {
                    returnType = method.Return.GetDomainType(registry);
                }
            }

            return returnType;
        }
    }
}
