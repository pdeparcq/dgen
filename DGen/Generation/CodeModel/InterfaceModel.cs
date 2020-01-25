using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DGen.Generation.CodeModel
{
    public class InterfaceModel : TypeModel
    {
        public List<InterfaceModel> ImplementedInterfaces { get; }
        public List<TypeModel> GenericTypes { get; }
        public List<InterfaceModel> Attributes { get; }
        public List<PropertyModel> Properties { get; }
        public List<MethodModel> Methods { get; }
        public bool IsGeneric => GenericTypes.Any();

        public virtual IEnumerable<NamespaceModel> Usings
        {
            get
            {
                var usings = Properties
                    .SelectMany(p => p.Usings)
                    .Concat(ImplementedInterfaces.Select(i => i.Namespace))
                    .Concat(ImplementedInterfaces.SelectMany(i => i.Usings))
                    .Concat(Methods.SelectMany(m => m.Usings))
                    .Concat(GenericTypes.Select(t => t.Namespace))
                    .Concat(Attributes.Select(t => t.Namespace))
                    .ToList();

                return usings.Where(n => n != Namespace && !Namespace.HasParent(n)).Distinct();
            }
        }

        public InterfaceModel(NamespaceModel @namespace, string name)
            : base(@namespace, name)
        {
            ImplementedInterfaces = new List<InterfaceModel>();
            GenericTypes = new List<TypeModel>();
            Attributes = new List<InterfaceModel>();
            Properties = new List<PropertyModel>();
            Methods = new List<MethodModel>();
        }


        public InterfaceModel WithImplementedInterfaces(params InterfaceModel[] interfaces)
        {
            ImplementedInterfaces.AddRange(interfaces);

            return this;
        }

        public InterfaceModel WithGenericTypes(params TypeModel[] types)
        {
            GenericTypes.AddRange(types);

            return this;
        }

        public InterfaceModel WithAttributes(params InterfaceModel[] attributes)
        {
            Attributes.AddRange(attributes);

            return this;
        }

        public PropertyModel AddProperty(string name, TypeModel type)
        {
            var property = Properties.FirstOrDefault(p => p.Name == name && p.Type == type);
            if(property == null)
            {
                property = new PropertyModel(this, name, type);
                Properties.Add(property);
            }
            return property;
        }

        public bool HasProperty(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return GetProperty(name, comparisonType) != null;
        }

        public virtual PropertyModel GetProperty(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return Properties.SingleOrDefault(p => p.Name.Equals(name, comparisonType));
        }

        public virtual MethodModel AddMethod(string name)
        {
            var method = new MethodModel(this, name);
            Methods.Add(method);
            return method;
        }

        public bool HasMethod(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return GetMethod(name, comparisonType) != null;
        }

        public virtual MethodModel GetMethod(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return Methods.SingleOrDefault(m => m.Name.Equals(name, comparisonType));
        }

        public override TypeSyntax Syntax
        {
            get
            {
                if (GenericTypes.Any())
                {
                    return SyntaxFactory.ParseTypeName($"{Name}<{string.Join(",", GenericTypes.Select(t => t.Name))}>");
                }
                return base.Syntax;
            }
        }
        
    }
}
