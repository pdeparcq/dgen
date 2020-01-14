namespace DGen.Meta.MetaModel.Types
{
    public class Command : BaseType
    {
        public Service Service { get; set; }
        public string ServiceMethod { get; set; }
    }
}
