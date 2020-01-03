namespace DGen.Meta.MetaModel.Types
{
    public class Query : BaseType
    {
        public ViewModel Result { get; set; }
        public bool IsCollection { get; set; }
    }
}