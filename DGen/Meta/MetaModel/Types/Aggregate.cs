using System.Collections.Generic;

namespace DGen.Meta.MetaModel.Types
{
    public class Aggregate : Entity
    {
        public Aggregate()
        {
            DomainEvents = new List<DomainEvent>();
        }

        public List<DomainEvent> DomainEvents { get; set; }
    }
}