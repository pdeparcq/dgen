﻿using System;
using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Application
{
    public class CommandHandlerCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Application";


        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.GetTypes<Command>();
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
                var commandType = registry.Resolve(Layer, command);

                @class = @class.WithImplementedInterfaces(SystemTypes.CommandHandler(commandType));

                var handler = @class.AddMethod("Handle")
                    .WithParameters(new MethodParameter("command", commandType))
                    .WithReturnType(SystemTypes.CommandResponse())
                    .WithBody(builder => { builder.ThrowNotImplemented(); });
            }
        }

        
    }
}