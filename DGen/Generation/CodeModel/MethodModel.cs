using System;
using Guards;
using System.Collections.Generic;
using System.Linq;
using DGen.Generation.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace DGen.Generation.CodeModel
{
    public class MethodModel
    {
        public InterfaceModel Interface { get; }
        public string Name { get; }
        public TypeModel ReturnType { get; private set; }
        public List<MethodParameter> Parameters { get; }
        public List<InterfaceModel> Attributes { get; }
        public List<StatementSyntax> Body { get; private set; }
        public List<TypeModel> UsedTypes { get; }
        public ExpressionSyntax Expression => SyntaxFactory.IdentifierName(Name);
        public bool IsAbstract => Body == null && !IsVirtual;
        public bool IsVirtual { get; private set; }
        public Accessibility Accessability { get; private set; }

        public IEnumerable<NamespaceModel> Usings
        {
            get
            {
                var usings = Attributes.Select(t => t.Namespace)
                    .Concat(Parameters.Select(p => p.Type.Namespace))
                    .Concat(UsedTypes.Select(t => t.Namespace))
                    .ToList();

                if (ReturnType != null)
                    usings.Add(ReturnType.Namespace);

                return usings;
            }
        }

        public MethodModel(InterfaceModel @interface, string name)
        {
            Guard.ArgumentNotNull(() => @interface);
            Guard.ArgumentNotNullOrEmpty(() => name);

            Interface = @interface;
            Name = name;
            Parameters = new List<MethodParameter>();
            Attributes = new List<InterfaceModel>();
            UsedTypes = new List<TypeModel>();
            Accessability = Accessibility.NotApplicable;
        }

        public MethodModel MakeVirtual()
        {
            IsVirtual = true;

            return this;
        }

        public MethodModel MakePublic()
        {
            Accessability = Accessibility.Public;

            return this;
        }

        public MethodModel MakeProtected()
        {
            Accessability = Accessibility.Protected;

            return this;
        }

        public MethodModel MakePrivate()
        {
            Accessability = Accessibility.Private;

            return this;
        }

        public ExpressionSyntax Invoke(params ExpressionSyntax[] parameters)
        {
            return SyntaxFactory.InvocationExpression(Expression, parameters.ToArgumentList());
        }

        public MethodModel WithReturnType(TypeModel returnType)
        {
            ReturnType = returnType;

            return this;
        }

        public MethodModel UseType(TypeModel type)
        {
            if (!UsedTypes.Contains(type))
            {
                UsedTypes.Add(type);
            }

            return this;
        }

        public MethodModel WithParameters(params MethodParameter[] parameters)
        {
            Parameters.AddRange(parameters);

            return this;
        }

        public MethodModel WithPropertyParameters()
        {
            return WithParameters(Interface.Properties.Select(p => new MethodParameter(p.Name.ToCamelCase(), p.Type))
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

        public MethodModel WithAttributes(params InterfaceModel[] attributes)
        {
            Attributes.AddRange(attributes);

            return this;
        }

        public MethodModel WithBody(Action<BodyBuilder> build)
        {
            var builder = new BodyBuilder(this);
            build(builder);
            Body = builder.Statements;

            return this;
        }
    }
}
