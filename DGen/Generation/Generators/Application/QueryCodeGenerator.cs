using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Application
{
    public class QueryCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Application";

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.GetTypes<Query>();
        }

        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Queries");
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if(type is Query query && model is ClassModel @class && query.Result is ViewModel viewModel)
            {
                @class = @class.WithBaseType(SystemTypes.Query(registry.Resolve(Layer, viewModel)));
                foreach(var property in query.Properties)
                {
                    @class.AddDomainProperty(property, registry);
                }
            }
        }
    }
}
