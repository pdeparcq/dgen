using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Application
{
    public class CommandHandlerCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Application";

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.GetTypes<Command>().Where(c => c.Service != null);
        }

        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Commands").AddNamespace("Handlers");
        }

        public override string GetTypeName(BaseType type)
        {
            return $"{type.Name}CommandHandler";
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if(type is Command command && model is ClassModel @class)
            {
                var serviceInterface = registry.Resolve(Layer, command.Service, $"I{command.Service.Name}");

                var commandType = registry.Resolve(Layer, command);

                @class = @class.WithImplementedInterfaces(SystemTypes.CommandHandler(commandType));

                @class.AddProperty(command.Service.Name, serviceInterface).MakeReadOnly();

                @class.AddConstructor()
                    .WithPropertyParameters()
                    .WithBody(builder =>
                    {
                        builder.AssignPropertiesFromParameters();
                    });

                var handler = @class.AddMethod("Handle")
                    .WithParameters(new MethodParameter("command", commandType))
                    .WithReturnType(SystemTypes.CommandResponse())
                    .WithBody(builder => 
                    {
                        if(command.ServiceMethod != null)
                        {
                            var method = command.Service.Methods.SingleOrDefault(m => m.Name == command.ServiceMethod);

                            if(method != null)
                            {
                                builder.InvokePropertyMethod(command.Service.Name, command.ServiceMethod);
                                builder.Return(SystemTypes.CommandResponse().Construct());
                            }
                            else
                            {
                                builder.ThrowNotImplemented();
                            }
                        }
                        else
                        {
                            builder.ThrowNotImplemented();
                        }     
                    });
            }
        }

        
    }
}
