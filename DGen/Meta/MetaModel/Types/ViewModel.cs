namespace DGen.Meta.MetaModel.Types
{
    public class ViewModel : BaseType
    {
        public BaseType Target { get; set; }
        public bool IsCompact { get; set; }
    }
}