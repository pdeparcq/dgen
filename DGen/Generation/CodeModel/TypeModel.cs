using System;
using Guards;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DGen.Generation.CodeModel
{
    public abstract class TypeModel : IEquatable<TypeModel>
    {
        public NamespaceModel Namespace { get; }
        public string Name { get; }
        public string FullName => $"{Namespace.FullName}.{Name}";
        public string Description { get; private set; }
        public ExpressionSyntax Expression => SyntaxFactory.IdentifierName(Name);
        public virtual TypeSyntax Syntax => SyntaxFactory.ParseTypeName(Name);

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

        public bool Equals(TypeModel other)
        {
            if (other == null)
                return false;

            return FullName == other.FullName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeModel) obj);
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        public static bool operator ==(TypeModel type1, TypeModel type2)
        {
            if (ReferenceEquals(type1, type2))
            {
                return true;
            }

            if (ReferenceEquals(type1, null))
            {
                return false;
            }
            if (ReferenceEquals(type2, null))
            {
                return false;
            }

            return type1.Equals(type2);
        }

        public static bool operator !=(TypeModel type1, TypeModel type2)
        {
            return !(type1 == type2);
        }
    }
}
