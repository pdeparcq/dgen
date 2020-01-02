using System;
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

        public BodyBuilder ReturnNull()
        {
            return Return(SyntaxFactory.ParseExpression("null"));
        }

        public BodyBuilder ThrowNotImplemented()
        {
            AddStatement(SyntaxFactory.ThrowStatement(SyntaxFactory.ParseExpression("new System.NotImplementedException()")));

            return this;
        }

        public BodyBuilder AssignProperties()
        {
            foreach (var parameter in Method.Parameters)
            {
                AssignProperty(parameter.Name);
            }

            return this;
        }

        public BodyBuilder AssignProperty(string propertyName, string parameterName = null)
        {
            if(Method.Class.HasProperty(propertyName) && Method.HasParameter(parameterName ?? propertyName))
                return Assign(Method.Class.GetProperty(propertyName).Expression, Method.GetParameter(parameterName ?? propertyName).Expression);

            return this;
        }

        public BodyBuilder InvokeMethod(string methodName, params ExpressionSyntax[] parameters)
        {
            throw new NotImplementedException();
        }

        private void AddStatement(StatementSyntax statement)
        {
            Statements.Add(statement);
        }

        private BodyBuilder Assign(ExpressionSyntax left, ExpressionSyntax right)
        {
            AddStatement(SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, right)));

            return this;
        }

        private BodyBuilder Return(ExpressionSyntax expression = null)
        {
            AddStatement(SyntaxFactory.ReturnStatement(expression));

            return this;
        }
    }
}