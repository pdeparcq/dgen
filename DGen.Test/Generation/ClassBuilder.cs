﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Generic;

namespace DGen.Test.Generation
{
    public class ClassBuilder
    {
        private readonly SyntaxGenerator _generator;
        private readonly List<SyntaxNode> _usings;
        private NamespaceDeclarationSyntax _namespace;
        private ClassDeclarationSyntax _class;

        public ClassBuilder(SyntaxGenerator generator, string namespaceName, string className)
        {
            _generator = generator;
            _usings = new List<SyntaxNode>();
            _class = _generator.ClassDeclaration(className) as ClassDeclarationSyntax;
            _namespace = _generator.NamespaceDeclaration(namespaceName) as NamespaceDeclarationSyntax;
        }

        public ClassBuilder AddBaseType(string type)
        {
            _class = _generator.AddBaseType(_class, _generator.IdentifierName(type)) as ClassDeclarationSyntax;

            return this;
        }

        public ClassBuilder AddNamespaceImportDeclaration(string name)
        {
            _usings.Add(_generator.NamespaceImportDeclaration(name));

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
            _namespace = _namespace.AddMembers(_class);
            var declarations = new List<SyntaxNode>(_usings);
            declarations.Add(_namespace);
            var compilationUnit = _generator.CompilationUnit(declarations).NormalizeWhitespace();
            return compilationUnit.ToFullString();
        }
    }
}
