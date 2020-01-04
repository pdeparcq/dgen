namespace DGen.Meta.MetaModel.Types
{
    public class Command : BaseType
    {
        public InputModel Input { get; set; }
        public DomainEvent DomainEvent { get; set; }
        public string MethodName { get; set; }
        public bool IsCollection { get; set; }
    }
}
