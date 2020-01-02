using Guards;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DGen.Generation.CodeModel
{
    public class MethodParameter
    {
        public string Name { get; }
        public TypeModel Type { get; }

        public MethodParameter(string name, TypeModel type)
        {
            Guard.ArgumentNotNullOrEmpty(() => name);
            Guard.ArgumentNotNull(() => type);

            Name = name;
            Type = type;
        }

        public ExpressionSyntax Expression => SyntaxFactory.IdentifierName(Name);


        public ExpressionSyntax Property(string propertyName)
        {
            if (Type is ClassModel @class && @class.HasProperty(propertyName))
            {
                return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Expression,
                    @class.GetProperty(propertyName).Expression as SimpleNameSyntax);
            }

            return null;
        }
    }
}