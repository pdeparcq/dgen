using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation
{
    public class ClassBuilder
    {
        private static readonly Regex AutoPropRegex = new Regex(@"\s*\{\s*get;\s*set;\s*}\s");

        private readonly SyntaxGenerator _generator;
        private readonly List<SyntaxNode> _usings;
        private NamespaceDeclarationSyntax _namespace;
        private ClassDeclarationSyntax _class;

        public ClassBuilder(SyntaxGenerator generator, string namespaceName, string className)
        {
            _generator = generator;
            _usings = new List<SyntaxNode>();
            _class = _generator.ClassDeclaration(className, accessibility: Accessibility.Public) as ClassDeclarationSyntax;
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

            if (_usings.All(u => u != declaration))
            {
                _usings.Add(declaration);
            };

            return this;
        }
        public ClassBuilder AddAutoProperty(string name, string type)
        {
            var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(type), name)
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .AddAccessorListAccessors(
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            _class = _class.AddMembers(property);

            return this;
        }

        public override string ToString()
        {
            var ns = _namespace.AddMembers(_class);
            var declarations = new List<SyntaxNode>(_usings);
            declarations.Add(ns);
            var compilationUnit = _generator.CompilationUnit(declarations).NormalizeWhitespace();
            return FormatAutoPropertiesOnOneLine(compilationUnit.ToFullString());
        }

        private string FormatAutoPropertiesOnOneLine(string str)
        {
            return AutoPropRegex.Replace(str, " { get; set; }");
        }
    }
}
