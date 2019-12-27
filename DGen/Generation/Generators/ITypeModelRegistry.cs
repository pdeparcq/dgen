using DGen.Generation.CodeModel;
using DGen.Meta;
using System.Linq;

namespace DGen.Generation.Generators
{
    public interface ITypeModelRegistry
    {
        void Register(string layer, BaseType type, TypeModel model);
        TypeModel Resolve(string layer, BaseType type, string name = null);
        IQueryable<T> GetAllBaseTypes<T>(string layer) where T : BaseType;
    }
}
