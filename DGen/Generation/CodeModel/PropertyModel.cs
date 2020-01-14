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
        public TypeModel Type { get; }
        public bool IsReadOnly { get; set; }

        public ExpressionSyntax Expression => SyntaxFactory.IdentifierName(Name);

        public IEnumerable<NamespaceModel> Usings
        {
            get
            {
                if(Type is ClassModel @class && @class.IsGeneric)
                {
                    foreach(var type in @class.GenericTypes)
                    {
                        yield return type.Namespace;
                    }
                }
                yield return Type.Namespace;
            }
        }

        public PropertyModel(string name, TypeModel type)
        {
            Guard.ArgumentNotNullOrEmpty(() => name);
            Guard.ArgumentNotNull(() => type);

            Name = name;
            Type = type;
        }

        public PropertyModel WithDescription(string description)
        {
            Description = description;

            return this;
        }

        public PropertyModel MakeReadOnly()
        {
            IsReadOnly = true;

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
