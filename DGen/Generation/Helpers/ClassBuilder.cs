using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Helpers
{
    public class ClassBuilder
    {
        private readonly SyntaxGenerator _generator;
        private readonly List<SyntaxNode> _usings;
        private NamespaceDeclarationSyntax _namespace;
        private ClassDeclarationSyntax _class;

        public ClassBuilder(SyntaxGenerator generator, string namespaceName, string className, string description = null)
        {
            _generator = generator;
            _usings = new List<SyntaxNode>();
            _class = _generator.ClassDeclaration(className, accessibility: Accessibility.Public) as ClassDeclarationSyntax;
            if (description != null)
                _class = _class.WithLeadingTrivia(SyntaxFactory.Comment(description));
            _namespace = _generator.NamespaceDeclaration(namespaceName) as NamespaceDeclarationSyntax;
        }

        public ClassBuilder AddBaseType(string type)
        {
            _class = _generator.AddBaseType(_class, _generator.IdentifierName(type)) as ClassDeclarationSyntax;

            return this;
        }

        public ClassBuilder AddNamespaceImportDeclaration(string name)
        {
            var declaration = _generator.NamespaceImportDeclaration(name);

            if (_usings.All(u => u.ToString() != declaration.ToString()))
            {
                _usings.Add(declaration);
            };

            return this;
        }
        public ClassBuilder AddAutoProperty(string name, string type, bool readOnly = false)
        {
            var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(type), name)
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .AddAccessorListAccessors(
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            if (!readOnly)
            {
                property = property.AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            }

            _class = _class.AddMembers(property);

            return this;
        }

        public ClassBuilder AddMethod(MethodDeclarationSyntax method)
        {
            _class = _class.AddMembers(method);

            return this;
        }

        public SyntaxNode Build()
        {
            var ns = _namespace.AddMembers(_class);
            var declarations = new List<SyntaxNode>(_usings.OrderBy(u => u.ToString()));
            declarations.Add(ns);
            return _generator.CompilationUnit(declarations);
        }
    }
}
