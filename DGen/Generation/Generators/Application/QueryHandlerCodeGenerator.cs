using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Generators.Application
{
    public class QueryHandlerCodeGenerator : LayerCodeGenerator
    {
        public override string Layer => "Application";

        public override IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.GetTypes<Query>();
        }

        public override NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Queries").AddNamespace("Handlers");
        }

        public override string GetTypeName(BaseType type)
        {
            return $"{type.Name}QueryHandler";
        }

        public override void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public override void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is Query query && model is ClassModel @class && query.Result is ViewModel viewModel)
            {
                var queryType = registry.Resolve(Layer, query);
                var queryResultType = registry.Resolve(Layer, viewModel);

                @class = @class.WithImplementedInterfaces(SystemTypes.QueryHandler(queryType, queryResultType));

                var handler = @class.AddMethod("Handle")
                    .WithParameters(new MethodParameter("query", queryType))
                    .WithReturnType(queryResultType)
                    .WithBody(builder => { builder.ThrowNotImplemented(); });
            }
        }
    }
}
