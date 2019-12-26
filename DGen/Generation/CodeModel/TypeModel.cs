using Guards;

namespace DGen.Generation.CodeModel
{
    public abstract class TypeModel
    {
        public NamespaceModel Namespace { get; }
        public string Name { get; }

        protected TypeModel(NamespaceModel @namespace, string name)
        {
            Guard.ArgumentNotNull(() => @namespace);
            Guard.ArgumentNotNullOrEmpty(() => name);

            Namespace = @namespace;
            Name = name;
        }
    }
}
