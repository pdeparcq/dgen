using DGen.Meta.MetaModel;
using DGen.StarUml;

namespace DGen.Meta
{
    public interface ITypeRegistry
    {
        void Register(Element e, BaseType type);
        BaseType Resolve(Element e);
    }
}