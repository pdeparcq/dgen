using Guards;

namespace DGen.Generation.CodeModel
{
    public class PropertyModel
    {
        public string Name { get; }
        public TypeModel Type { get; }
        public bool IsReadOnly { get; set; }

        public PropertyModel(string name, TypeModel type)
        {
            Guard.ArgumentNotNullOrEmpty(() => name);
            Guard.ArgumentNotNull(() => type);

            Name = name;
            Type = type;
        }

        public PropertyModel MakeReadOnly()
        {
            IsReadOnly = true;

            return this;
        }
    }
}
