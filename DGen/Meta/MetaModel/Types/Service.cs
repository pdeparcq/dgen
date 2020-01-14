using System.Collections.Generic;

namespace DGen.Meta.MetaModel.Types
{
    public class Service : BaseType
    {
        public Service()
        {
            QueryRepositories = new List<Aggregate>();
            Services = new List<Service>();
        }

        public Aggregate AggregateRepository { get; set; }
        public List<Aggregate> QueryRepositories { get; private set; }
        public List<Service> Services { get; private set; }
    }
}
