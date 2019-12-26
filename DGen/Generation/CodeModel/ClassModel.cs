using System.Collections.Generic;
using System.Linq;

namespace DGen.Generation.CodeModel
{
    public class ClassModel : TypeModel
    {
        public List<TypeModel> GenericTypes { get; }
        public List<PropertyModel> Properties { get; }
        public bool IsGeneric => GenericTypes.Any();

        public IEnumerable<NamespaceModel> Usings
        {
            get
            {
                return Properties
                    .SelectMany(p => p.Usings)
                    .Concat(GenericTypes.Select(t => t.Namespace))
                    .Where(n => n != Namespace)
                    .Distinct();
            }
        }

        public ClassModel(NamespaceModel @namespace, string name)
            : base(@namespace, name)
        {
            GenericTypes = new List<TypeModel>();
            Properties = new List<PropertyModel>();
        }

        public ClassModel WithGenericTypes(params TypeModel[] types)
        {
            GenericTypes.AddRange(types);

            return this;
        }

        public PropertyModel AddProperty(string name, TypeModel type)
        {
            var property = Properties.FirstOrDefault(p => p.Name == name && p.Type == type);
            if(property == null)
            {
                property = new PropertyModel(name, type);
                Properties.Add(property);
            }
            return property;
        }
    }
}
