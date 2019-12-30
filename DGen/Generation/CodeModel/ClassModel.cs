using System;
using System.Collections.Generic;
using System.Linq;

namespace DGen.Generation.CodeModel
{
    public class ClassModel : TypeModel
    {
        public ClassModel BaseType { get; private set; }
        public List<TypeModel> GenericTypes { get; }
        public List<ClassModel> Attributes { get; }
        public List<PropertyModel> Properties { get; }
        public List<MethodModel> Constructors { get; }
        public List<MethodModel> Methods { get; }
        public bool IsGeneric => GenericTypes.Any();

        public IEnumerable<NamespaceModel> Usings
        {
            get
            {
                var usings = Properties
                    .SelectMany(p => p.Usings)
                    .Concat(Constructors.SelectMany(m => m.Usings))
                    .Concat(Methods.SelectMany(m => m.Usings))
                    .Concat(GenericTypes.Select(t => t.Namespace))
                    .Concat(Attributes.Select(t => t.Namespace))
                    .ToList();

                if (BaseType != null)
                {
                    usings.AddRange(BaseType.Usings);
                    usings.Add(BaseType.Namespace);
                }

                return usings.Where(n => n != Namespace).Distinct();
            }
        }

        public ClassModel(NamespaceModel @namespace, string name)
            : base(@namespace, name)
        {
            GenericTypes = new List<TypeModel>();
            Attributes = new List<ClassModel>();
            Properties = new List<PropertyModel>();
            Constructors = new List<MethodModel>();
            Methods = new List<MethodModel>();
        }

        public ClassModel WithGenericTypes(params TypeModel[] types)
        {
            GenericTypes.AddRange(types);

            return this;
        }

        public ClassModel WithAttributes(params ClassModel[] attributes)
        {
            Attributes.AddRange(attributes);

            return this;
        }

        public ClassModel WithBaseType(ClassModel baseType)
        {
            BaseType = baseType;

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

        public bool HasProperty(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return GetProperty(name, comparisonType) != null;
        }

        public PropertyModel GetProperty(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return Properties.SingleOrDefault(p => p.Name.Equals(name, comparisonType));
        }

        public MethodModel AddConstructor()
        {
            var constructor = new MethodModel(this, Name);
            Constructors.Add(constructor);
            return constructor;
        }

        public MethodModel AddMethod(string name)
        {
            var method = new MethodModel(this, name);
            Methods.Add(method);
            return method;
        }

        public override string ToString()
        {
            if (GenericTypes.Any())
            {
                return $"{Name}<{string.Join(",", GenericTypes.Select(t => t.Name))}>";
            }
            else
            {
                return Name;
            }
        }
    }
}
