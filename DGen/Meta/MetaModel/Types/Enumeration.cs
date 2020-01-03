using System.Collections.Generic;

namespace DGen.Meta.MetaModel.Types
{
    public class Enumeration : BaseType
    {
        public Enumeration()
        {
            Literals = new List<string>();
        }

        public List<string> Literals { get; set; }
    }
}