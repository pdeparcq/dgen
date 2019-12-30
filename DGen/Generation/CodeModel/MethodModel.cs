using Guards;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

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
    }

    public class MethodModel
    {
        public string Name { get; }
        public TypeModel ReturnType { get; private set; }
        public List<MethodParameter> Parameters { get; }
        public List<ClassModel> Attributes { get; }
        public List<StatementSyntax> Statements { get; }

        public IEnumerable<NamespaceModel> Usings
        {
            get
            {
                var usings = Attributes.Select(t => t.Namespace)
                    .Concat(Parameters.Select(p => p.Type.Namespace))
                    .ToList();

                if (ReturnType != null)
                    usings.Add(ReturnType.Namespace);

                return usings;
            }
        }

        public MethodModel(string name)
        {
            Guard.ArgumentNotNullOrEmpty(() => name);

            Name = name;
            Parameters = new List<MethodParameter>();
            Attributes = new List<ClassModel>();
            Statements = new List<StatementSyntax>();
        }

        public MethodModel ReturnNull()
        {
            return Return(SyntaxFactory.ParseExpression("null"));
        }

        public MethodModel Return(ExpressionSyntax expression = null)
        {
            AddStatement(SyntaxFactory.ReturnStatement(expression));

            return this;
        }

        public MethodModel ThrowNotImplemented()
        {
            AddStatement(SyntaxFactory.ThrowStatement(SyntaxFactory.ParseExpression("new NotImplementedException()")));

            return this;
        }

        public MethodModel Assign(ExpressionSyntax left, ExpressionSyntax right)
        {
            AddStatement(SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, right)));

            return this;
        }

        public MethodModel WithReturnType(TypeModel returnType)
        {
            ReturnType = returnType;

            return this;
        }

        public MethodModel WithParameters(params MethodParameter[] parameters)
        {
            Parameters.AddRange(parameters);

            return this;
        }

        public MethodModel WithAttributes(params ClassModel[] attributes)
        {
            Attributes.AddRange(attributes);

            return this;
        }

        private void AddStatement(StatementSyntax statement)
        {
            Statements.Add(statement);
        }
    }
}
