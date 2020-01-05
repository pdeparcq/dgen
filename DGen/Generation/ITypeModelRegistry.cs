using DGen.Generation.CodeModel;
using System.Linq;
using DGen.Meta.MetaModel;

namespace DGen.Generation
{
    public interface ITypeModelRegistry
    {
        void Register(string layer, BaseType type, TypeModel model);
        TypeModel Resolve(string layer, BaseType type, string name = null);
        IQueryable<T> GetAllBaseTypes<T>(string layer) where T : BaseType;
    }
}
