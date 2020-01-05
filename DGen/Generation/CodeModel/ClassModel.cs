using System;
using System.Collections.Generic;
using System.Linq;
using DGen.Generation.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DGen.Generation.CodeModel
{
    public class ClassModel : InterfaceModel
    {
        public ClassModel BaseType { get; private set; }
        public List<MethodModel> Constructors { get; }
        public bool IsAbstract => Methods.Any(m => m.IsAbstract) || Constructors.Any(c => c.Accessability == Accessibility.Protected);

        public override IEnumerable<NamespaceModel> Usings
        {
            get
            {
                var usings = base.Usings
                    .Concat(Constructors.SelectMany(c => c.Usings))
                    .ToList();

                if (BaseType != null)
                {
                    usings.AddRange(BaseType.Usings);
                    usings.Add(BaseType.Namespace);
                }

                return usings.Where(n => n != Namespace).Distinct();
            }
        }

        public ClassModel(NamespaceModel @namespace, string name)
            : base(@namespace, name)
        {
            Constructors = new List<MethodModel>();
        }

        public ClassModel WithBaseType(ClassModel baseType)
        {
            BaseType = baseType;

            return this;
        }

        public new ClassModel WithImplementedInterfaces(params InterfaceModel[] interfaces)
        {
            return base.WithImplementedInterfaces(interfaces) as ClassModel;
        }

        public new ClassModel WithGenericTypes(params TypeModel[] types)
        {
            return base.WithGenericTypes(types) as ClassModel;
        }

        public new ClassModel WithAttributes(params ClassModel[] attributes)
        {
            return base.WithAttributes(attributes) as ClassModel;
        }

        public override PropertyModel GetProperty(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return base.GetProperty(name, comparisonType) ?? BaseType?.GetProperty(name, comparisonType);
        }

        public override MethodModel GetMethod(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return base.GetMethod(name, comparisonType) ?? BaseType?.GetMethod(name, comparisonType);
        }

        public MethodModel AddConstructor()
        {
            var constructor = new MethodModel(this, Name);
            Constructors.Add(constructor);
            return constructor;
        }

        public ExpressionSyntax Construct(params ExpressionSyntax[] parameters)
        {
            return Construct(parameters, new AssignmentExpressionSyntax[] { });
        }

        public ExpressionSyntax Construct(IEnumerable<ExpressionSyntax> parameters, params AssignmentExpressionSyntax[] initializer)
        {
            var expression = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(Name),
                parameters.ToArgumentList(), null);

            //Add initializer if provided
            if (initializer != null && initializer.Any())
            {
                var list = new SeparatedSyntaxList<ExpressionSyntax>();
                list = list.AddRange(initializer);
                expression = expression.WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression, list));
            }

            return expression;
        }

        public AssignmentExpressionSyntax Initializer(string name, ExpressionSyntax expression)
        {
            return SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, GetProperty(name).Expression, expression);
        }
    }
}
