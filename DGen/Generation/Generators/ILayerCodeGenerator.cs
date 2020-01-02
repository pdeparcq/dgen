using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta;

namespace DGen.Generation.Generators
{
    public interface ILayerCodeGenerator
    {
        string Layer { get; }
        IEnumerable<BaseType> GetTypes(Module module);
        string GetTypeName(BaseType type);
        NamespaceModel GetNamespace(NamespaceModel @namespace);
        TypeModel PrepareType(NamespaceModel @namespace, BaseType type);
        void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry);
        void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry);
    }
}