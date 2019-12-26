using DGen.Generation.CodeModel;
using DGen.Meta;
using System.Linq;

namespace DGen.Generation.Generators
{
    public interface ITypeModelRegistry
    {
        void Register(string layer, BaseType type, TypeModel model);
        TypeModel Resolve(string layer, BaseType type);
    }
}
