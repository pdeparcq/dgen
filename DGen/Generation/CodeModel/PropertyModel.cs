using Guards;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using DGen.Generation.Extensions;

namespace DGen.Generation.CodeModel
{
    public class PropertyModel
    {
        public string Name { get; }
        public string Description { get; private set; }
        public TypeModel Type => Getter.ReturnType;
        public MethodModel Getter { get; }
        public MethodModel Setter { get; }

        public ExpressionSyntax Expression => SyntaxFactory.IdentifierName(Name);

        public IEnumerable<NamespaceModel> Usings
        {
            get
            {
                if(Type is InterfaceModel @interface && @interface.IsGeneric)
                {
                    foreach(var type in @interface.GenericTypes)
                    {
                        yield return type.Namespace;
                    }
                }
                yield return Type.Namespace;
            }
        }

        public PropertyModel(InterfaceModel @interface, string name, TypeModel type)
        {
            Guard.ArgumentNotNullOrEmpty(() => name);
            Guard.ArgumentNotNull(() => type);

            Name = name;
            Getter = new MethodModel(@interface, $"Get{name}").WithReturnType(type).MakePublic();
            Setter = new MethodModel(@interface, $"Set{name}").WithParameters(new MethodParameter("value", type));
        }

        public PropertyModel WithDescription(string description)
        {
            Description = description;

            return this;
        }

        public PropertyModel WithGetter(Action<BodyBuilder> build)
        {
            Getter.WithBody(build);

            return this;
        }

        public PropertyModel WithSetter(Action<BodyBuilder> build)
        {
            Setter.WithBody(build);

            return this;
        }

        public PropertyModel MakeReadOnly()
        {
            Setter.MakePrivate();

            return this;
        }

        public ExpressionSyntax InvokeMethod(string methodName, ExpressionSyntax[] parameters)
        {
            if (Type is InterfaceModel @interface && @interface.GetMethod(methodName) is MethodModel method)
            {
                return SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Expression, method.Expression as SimpleNameSyntax), parameters.ToArgumentList());
            }
            return null;
        }
    }
}
