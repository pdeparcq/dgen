using System.Collections.Generic;

namespace DGen.Meta.MetaModel.Types
{
    public class Service : BaseType
    {
        public Service()
        {
            Repositories = new List<Aggregate>();
        }

        public List<Aggregate> Repositories { get; set; }
    }
}
