using System.Linq;

namespace DGen.Meta.MetaModel.Types
{
    public class Entity : BaseType
    {
        public Property UniqueIdentifier => Properties?.FirstOrDefault(p => p.IsIdentifier);
    }
}