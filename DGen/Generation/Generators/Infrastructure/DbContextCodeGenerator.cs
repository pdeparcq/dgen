﻿using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Meta;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Infrastructure
{
    public class DbContextCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Infrastructure";

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.GetTypes<Aggregate>();
        }

        public override string GetTypeName(BaseType type)
        {
            return $"{type.Name}Data";
        }

        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Entities");
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            if (GetTypes(module).Any())
                @namespace.AddClass($"{module.Name}DbContext");
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            
        }
    }
}
