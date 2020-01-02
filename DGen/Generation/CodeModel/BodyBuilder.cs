using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DGen.Generation.CodeModel
{
    public class BodyBuilder
    {
        private MethodModel Method { get; }
        public List<StatementSyntax> Statements { get; }

        public BodyBuilder(MethodModel method)
        {
            Method = method;
            Statements = new List<StatementSyntax>();
        }

        public BodyBuilder AssignPropertiesFromParameters()
        {
            foreach (var parameter in Method.Parameters)
            {
                AssignPropertyFromParameter(parameter.Name);
            }

            return this;
        }

        public BodyBuilder AssignPropertyFromParameter(string propertyName, string parameterName = null)
        {
            if(Method.HasParameter(parameterName ?? propertyName))
                return AssignProperty(propertyName, Method.GetParameter(parameterName ?? propertyName).Expression);

            return this;
        }

        public BodyBuilder AssignProperty(string propertyName, ExpressionSyntax expression)
        {
            if (Method.Class.HasProperty(propertyName))
                return Assign(Method.Class.GetProperty(propertyName).Expression, expression);

            return this;
        }

        public BodyBuilder InvokeMethod(string methodName, params ExpressionSyntax[] parameters)
        {
            if (Method.Class.HasMethod(methodName))
            {
                return Execute(Method.Class.GetMethod(methodName).Invoke(parameters));
            }

            return this;
        }

        public BodyBuilder ThrowNotImplemented()
        {
            return Throw(SystemTypes.NotImplementedException());
        }

        private BodyBuilder Assign(ExpressionSyntax left, ExpressionSyntax right)
        {
            return Execute(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, right));
        }

        private BodyBuilder Execute(ExpressionSyntax expression)
        {
            return AddStatement(SyntaxFactory.ExpressionStatement(expression));
        }

        private BodyBuilder Throw(ClassModel exception, params ExpressionSyntax[] parameters)
        {
            Method.UseType(exception);
            return AddStatement(SyntaxFactory.ThrowStatement(exception.Construct(parameters)));
        }

        public BodyBuilder Return(ExpressionSyntax expression = null)
        {
            return AddStatement(SyntaxFactory.ReturnStatement(expression));
        }

        private BodyBuilder AddStatement(StatementSyntax statement)
        {
            Statements.Add(statement);

            return this;
        }
    }
}