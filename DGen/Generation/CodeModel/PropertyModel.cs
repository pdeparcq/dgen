using Guards;
using System.Collections.Generic;

namespace DGen.Generation.CodeModel
{
    public class PropertyModel
    {
        public string Name { get; }
        public TypeModel Type { get; }
        public bool IsReadOnly { get; set; }

        public IEnumerable<NamespaceModel> Usings
        {
            get
            {
                if(Type is ClassModel @class && @class.IsGeneric)
                {
                    foreach(var type in @class.GenericTypes)
                    {
                        yield return type.Namespace;
                    }
                }
                yield return Type.Namespace;
            }
        }

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
