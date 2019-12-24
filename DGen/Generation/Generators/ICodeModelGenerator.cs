using DGen.Generation.CodeModel;
using DGen.Meta;
using System.Collections.Generic;

namespace DGen.Generation.Generators
{
    public interface ICodeModelGenerator
    {
        string Layer { get; }
        IEnumerable<BaseType> GetTypes(Module module);
        void Visit(Module module, NamespaceModel @namespace);
        void Visit(BaseType type, NamespaceModel @namespace);
    }
}