using DGen.Generation.CodeModel;
using DGen.Meta;
using System.Collections.Generic;

namespace DGen.Generation.Generators
{
    public interface ICodeModelGenerator
    {
        string Layer { get; }
        IEnumerable<BaseType> GetTypes(Module module);
        string GetTypeName(BaseType type) => type.Name;
        NamespaceModel GetNamespace(NamespaceModel @namespace) => @namespace;
        TypeModel PrepareType(NamespaceModel @namespace, BaseType type) => GetNamespace(@namespace).AddClass(GetTypeName(type));
        void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry);
        void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry);
    }
}