using System;
using Guards;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using DGen.Generation.Extensions;

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
        public ClassModel Class { get; }
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

        public MethodModel(ClassModel @class, string name)
        {
            Guard.ArgumentNotNull(() => @class);
            Guard.ArgumentNotNullOrEmpty(() => name);

            Class = @class;
            Name = name;
            Parameters = new List<MethodParameter>();
            Attributes = new List<ClassModel>();
            Statements = new List<StatementSyntax>();
        }

        public MethodModel ReturnNull()
        {
            return Return(SyntaxFactory.ParseExpression("null"));
        }

        public MethodModel ThrowNotImplemented()
        {
            AddStatement(SyntaxFactory.ThrowStatement(SyntaxFactory.ParseExpression("new System.NotImplementedException()")));

            return this;
        }

        /*
         * Assign properties from parameters with same name
         */
        public MethodModel AssignProperties()
        {
            foreach (var parameter in Parameters)
            {
                AssignProperty(parameter.Name);
            }

            return this;
        }

        public MethodModel AssignProperty(string propertyName, string parameterName = null)
        {
            if(Class.HasProperty(propertyName) && HasParameter(parameterName ?? propertyName))
                return Assign(Class.GetProperty(propertyName).Expression, GetParameter(parameterName ?? propertyName).Expression);

            return this;
        }

        public MethodModel InvokeMethod(string methodName, params ExpressionSyntax[] parameters)
        {
            throw new NotImplementedException();
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

        /*
         * Generate method parameters from class properties
         */
        public MethodModel WithPropertyParameters()
        {
            return WithParameters(Class.Properties.Select(p => new MethodParameter(p.Name.ToCamelCase(), p.Type))
                .ToArray());
        }

        public bool HasParameter(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return GetParameter(name, comparisonType) != null;
        }

        public MethodParameter GetParameter(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return Parameters.SingleOrDefault(p => p.Name.Equals(name, comparisonType));
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

        private MethodModel Assign(ExpressionSyntax left, ExpressionSyntax right)
        {
            AddStatement(SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, right)));

            return this;
        }

        private MethodModel Return(ExpressionSyntax expression = null)
        {
            AddStatement(SyntaxFactory.ReturnStatement(expression));

            return this;
        }
    }
}
