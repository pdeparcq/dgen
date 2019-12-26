using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Generation.Extensions;
using DGen.Meta;

namespace DGen.Generation.Generators.Application
{
    public class QueryCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Application";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Queries;
        }

        public NamespaceModel GetNamespace(NamespaceModel @namespace)
        {
            return @namespace.AddNamespace("Queries");
        }

        public void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry)
        {
            
        }

        public void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry)
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
