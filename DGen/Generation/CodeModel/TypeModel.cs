using Guards;

namespace DGen.Generation.CodeModel
{
    public abstract class TypeModel
    {
        public NamespaceModel Namespace { get; }
        public string Name { get; }
        public string Description { get; private set; }

        protected TypeModel(NamespaceModel @namespace, string name)
        {
            Guard.ArgumentNotNull(() => @namespace);
            Guard.ArgumentNotNullOrEmpty(() => name);

            Namespace = @namespace;
            Name = name;
        }

        public TypeModel WithDescription(string description)
        {
            Description = description;

            return this;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
