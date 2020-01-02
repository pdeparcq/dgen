using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DGen.Generation.Extensions
{
    public static class ExpressionExtensions
    {
        public static ArgumentListSyntax ToArgumentList(this IEnumerable<ExpressionSyntax> parameters)
        {
            var argumentList = SyntaxFactory.ArgumentList();
            if (parameters.Any())
            {
                var arguments = new SeparatedSyntaxList<ArgumentSyntax>();
                arguments = arguments.AddRange(parameters.Select(SyntaxFactory.Argument));
                argumentList = SyntaxFactory.ArgumentList(arguments);
            }
            return argumentList;
        }

        public static IEnumerable<ExpressionSyntax> ToExpressions(this IEnumerable<MethodParameter> parameters)
        {
            return parameters.Select(p => p.Expression).ToArray();
        }
    }
}
