namespace DGen.Meta.MetaModel.Types
{
    public class Command : BaseType
    {
        public InputModel Input { get; set; }
        public Aggregate Aggregate { get; set; }
        public bool IsCollection { get; set; }
    }
}
