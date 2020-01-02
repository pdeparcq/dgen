using DGen.Generation.CodeModel;
using DGen.Meta;
using System.Collections.Generic;

namespace DGen.Generation.Generators
{
    public abstract class LayerCodeGenerator : ILayerCodeGenerator
    {
        public abstract string Layer { get; }
        public abstract IEnumerable<BaseType> GetTypes(Module module);
        public virtual string GetTypeName(BaseType type) => type.Name;
        public virtual NamespaceModel GetNamespace(NamespaceModel @namespace) => @namespace;
        public virtual TypeModel PrepareType(NamespaceModel @namespace, BaseType type) => GetNamespace(@namespace).AddClass(GetTypeName(type));
        public abstract void GenerateModule(Module module, NamespaceModel @namespace, ITypeModelRegistry registry);
        public abstract void GenerateType(BaseType type, TypeModel model, ITypeModelRegistry registry);
    }
}