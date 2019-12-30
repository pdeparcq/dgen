using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta;
using Microsoft.CodeAnalysis.CSharp;

namespace DGen.Generation.Generators.Application
{
    public class QueryHandlerCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Application";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Queries;
        }

        public NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Queries").AddNamespace("Handlers");
        }

        public string GetTypeName(BaseType type)
        {
            return $"{type.Name}QueryHandler";
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
        {
            if (type is Query query && model is ClassModel @class && query.Result is ViewModel viewModel)
            {
                var queryType = registry.Resolve(Layer, query);
                var queryResultType = registry.Resolve(Layer, viewModel);

                @class = @class.WithBaseType(SystemTypes.QueryHandler(queryType, queryResultType));

                var handler = @class.AddMethod("Handle")
                    .WithParameters(new MethodParameter("query", queryType))
                    .WithReturnType(queryResultType);

                handler.ThrowNotImplemented();
            }
        }
    }
}
