namespace DGen.Meta.MetaModel.Types
{
    public enum DomainEventType
    {
        Create,
        Update,
        Add,
        Remove,
        Delete
    }

    public class DomainEvent : BaseType
    {
        public Aggregate Aggregate { get; set; }
        public DomainEventType Type { get; set; }
    }
}