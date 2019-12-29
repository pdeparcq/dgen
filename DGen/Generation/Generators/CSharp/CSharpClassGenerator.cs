using DGen.Generation.CodeModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DGen.Generation.Generators.CSharp
{
    public class CSharpClassGenerator
    {
        private static readonly Regex AutoPropRegex = new Regex(@"\s*\{\s*get;\s*set;\s*}\s");
        private static readonly Regex AutoPropReadOnlyRegex = new Regex(@"\s*\{\s*get;\s*private set;\s*}\s");

        private readonly SyntaxGenerator _syntaxGenerator;

        public CSharpClassGenerator(SyntaxGenerator syntaxGenerator)
        {
            _syntaxGenerator = syntaxGenerator;
        }

        public string Generate(ClassModel model)
        {
            var @namespace = _syntaxGenerator.NamespaceDeclaration(model.Namespace.FullName) as NamespaceDeclarationSyntax;
            var @class = _syntaxGenerator.ClassDeclaration(model.Name, accessibility: Accessibility.Public) as ClassDeclarationSyntax;

            if (model.Description != null)
                @class = @class.WithLeadingTrivia(ToDocumentation(model.Description));

            @class = GenerateBaseType(model, @class);
            @class = GenerateTypeAttributes(model, @class);
            @class = GenerateConstructors(model, @class);
            @class = GenerateProperties(model, @class);
            @class = GenerateMethods(model, @class);

            var compileUnit = GenerateCompileUnit(model, @class, @namespace);

            return FormatAutoPropertiesOnOneLine(compileUnit.NormalizeWhitespace().ToFullString());
        }

        private ClassDeclarationSyntax GenerateConstructors(ClassModel model, ClassDeclarationSyntax @class)
        {
            foreach (var c in model.Constructors)
            {
                var constructor = _syntaxGenerator.ConstructorDeclaration(c.Name) as ConstructorDeclarationSyntax;

                constructor = GenerateMemberAttributes(c, constructor);

                constructor = constructor.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                foreach (var p in c.Parameters)
                {
                    var parameter = _syntaxGenerator.ParameterDeclaration(p.Name, SyntaxFactory.ParseTypeName(p.Type.ToString())) as ParameterSyntax;
                    constructor = constructor.AddParameterListParameters(parameter);
                }

                @class = @class.AddMembers(constructor);
            }
            return @class;
        }

        private ClassDeclarationSyntax GenerateMethods(ClassModel model, ClassDeclarationSyntax @class)
        {
            foreach (var m in model.Methods)
            {
                var method = _syntaxGenerator.MethodDeclaration(m.Name) as MethodDeclarationSyntax;

                method = GenerateMemberAttributes(m, method);

                method = method.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                if (m.ReturnType != null)
                    method = method.WithReturnType(SyntaxFactory.ParseTypeName(m.ReturnType.ToString()));

                foreach(var p in m.Parameters)
                {
                    var parameter = _syntaxGenerator.ParameterDeclaration(p.Name, SyntaxFactory.ParseTypeName(p.Type.ToString())) as ParameterSyntax;
                    method = method.AddParameterListParameters(parameter);
                }

                @class = @class.AddMembers(method);
            }
            return @class;
        }

        private SyntaxNode GenerateCompileUnit(ClassModel model, ClassDeclarationSyntax @class, NamespaceDeclarationSyntax @namespace)
        {
            @namespace = @namespace.AddMembers(@class);
            var declarations = model.Usings.OrderBy(u => u.FullName).Select(u => _syntaxGenerator.NamespaceImportDeclaration(u.FullName)).ToList();
            declarations.Add(@namespace);

            return _syntaxGenerator.CompilationUnit(declarations);
        }

        private ClassDeclarationSyntax GenerateProperties(ClassModel model, ClassDeclarationSyntax @class)
        {
            foreach (var p in model.Properties)
            {
                var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(p.Type.ToString()), p.Name)
                                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                    .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
                
                var setter = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)) as AccessorDeclarationSyntax;
                
                if (p.IsReadOnly)
                    setter = setter.AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

                property = property.AddAccessorListAccessors(setter);

                if (p.Description != null)
                    property = property.WithLeadingTrivia(ToDocumentation(p.Description));

                @class = @class.AddMembers(property);
            }
            return @class;
        }

        private ClassDeclarationSyntax GenerateBaseType(ClassModel model, ClassDeclarationSyntax @class)
        {
            if (model.BaseType != null)
            {
                @class = _syntaxGenerator.AddBaseType(@class, _syntaxGenerator.IdentifierName(model.BaseType.ToString())) as ClassDeclarationSyntax;
            }
            return @class;
        }

        private T GenerateTypeAttributes<T>(ClassModel model, T type) where T : TypeDeclarationSyntax
        {
            foreach (var a in model.Attributes)
            {
                type = type.AddAttributeLists(GenerateAttributeList(a)) as T;
            }
            return type;
        }

        private T GenerateMemberAttributes<T>(MethodModel model, T member) where T : MemberDeclarationSyntax
        {
            foreach (var a in model.Attributes)
            {
                member = member.AddAttributeLists(GenerateAttributeList(a)) as T;
            }
            return member;
        }

        private static AttributeListSyntax GenerateAttributeList(ClassModel a)
        {
            var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(a.Name)) as AttributeSyntax;
            var attributeList = SyntaxFactory.AttributeList() as AttributeListSyntax;
            attributeList = attributeList.AddAttributes(attribute);
            return attributeList;
        }

        private static SyntaxTrivia ToDocumentation(string text)
        {
            return SyntaxFactory.Comment($"/* {text} */");
        }

        private string FormatAutoPropertiesOnOneLine(string str)
        {
            str = AutoPropRegex.Replace(str, " { get; set; }");
            str = AutoPropReadOnlyRegex.Replace(str, " { get; private set; }");
            return str;
        }
    }
}
