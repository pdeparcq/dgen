using Guards;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DGen.Generation.CodeModel
{
    public abstract class TypeModel
    {
        public NamespaceModel Namespace { get; }
        public string Name { get; }
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
    }
}
