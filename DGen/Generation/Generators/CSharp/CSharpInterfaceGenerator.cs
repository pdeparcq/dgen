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
    public class CSharpInterfaceGenerator<I, T> where I : InterfaceModel where T : TypeDeclarationSyntax
    {
        private static readonly Regex AutoPropRegex = new Regex(@"\s*\{\s*get;\s*set;\s*}\s");
        private static readonly Regex AutoPropReadOnlyRegex = new Regex(@"\s*\{\s*get;\s*private set;\s*}\s");

        protected readonly SyntaxGenerator _syntaxGenerator;

        public CSharpInterfaceGenerator(SyntaxGenerator syntaxGenerator)
        {
            _syntaxGenerator = syntaxGenerator;
        }

        public string Generate(I model)
        {
            var @namespace = _syntaxGenerator.NamespaceDeclaration(model.Namespace.FullName) as NamespaceDeclarationSyntax;
            var @interface = GenerateInterface(model);
            var compileUnit = GenerateCompileUnit(model, @interface, @namespace);
            return FormatAutoPropertiesOnOneLine(compileUnit.NormalizeWhitespace().ToFullString());
        }

        protected virtual T GenerateInterface(I model)
        {
            var @interface = GenerateDeclaration(model);

            if (model.Description != null)
                @interface = @interface.WithLeadingTrivia(ToDocumentation(model.Description));

            @interface = GenerateImplementedInterfaces(model, @interface);
            @interface = GenerateTypeAttributes(model, @interface);
            @interface = GenerateProperties(model, @interface);
            @interface = GenerateMethods(model, @interface);

            return @interface;
        }

        protected virtual T GenerateDeclaration(I model)
        {
            return _syntaxGenerator.InterfaceDeclaration(model.Name, accessibility: Accessibility.Public) as T;
        }

        protected virtual T GenerateMethods(I model, T @interface)
        {
            foreach (var m in model.Methods)
            {
                var method = _syntaxGenerator.MethodDeclaration(m.Name, accessibility: m.Accessability, returnType: m.ReturnType?.Syntax) as MethodDeclarationSyntax;

                method = GenerateMethod(m, method);

                @interface = @interface.AddMembers(method) as T;
            }
            return @interface;
        }

        protected virtual M GenerateMethod<M>(MethodModel model, M method) where M : BaseMethodDeclarationSyntax
        {
            method = GenerateMemberAttributes(model, method);

            foreach (var p in model.Parameters)
            {
                var parameter = _syntaxGenerator.ParameterDeclaration(p.Name, p.Type.Syntax) as ParameterSyntax;
                method = method.AddParameterListParameters(parameter) as M;
            }

            if (!model.IsAbstract)
            {
                if (model.Body != null)
                    method = method.AddBodyStatements(model.Body.ToArray()) as M;
            }
            else
            {
                method = method.WithBody(null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)) as M;
            }

            return method;
        }

        private SyntaxNode GenerateCompileUnit(I model, T @interface, NamespaceDeclarationSyntax @namespace)
        {
            @namespace = @namespace.AddMembers(@interface);
            var declarations = model.Usings.OrderBy(u => u.FullName).Select(u => _syntaxGenerator.NamespaceImportDeclaration(u.FullName)).ToList();
            declarations.Add(@namespace);

            return _syntaxGenerator.CompilationUnit(declarations);
        }

        private T GenerateProperties(I model, T @interface)
        {
            foreach (var p in model.Properties)
            {
                var property = _syntaxGenerator.PropertyDeclaration(p.Name, p.Type.Syntax, p.Getter.Accessability) as PropertyDeclarationSyntax;

                property = property.WithAccessorList(null);
                property = property.AddAccessorListAccessors(_syntaxGenerator.GetAccessorDeclaration(Accessibility.NotApplicable, p.Getter.Body) as AccessorDeclarationSyntax);
                property = property.AddAccessorListAccessors(_syntaxGenerator.SetAccessorDeclaration(p.Setter.Accessability, p.Setter.Body) as AccessorDeclarationSyntax);

                if (p.Description != null)
                    property = property.WithLeadingTrivia(ToDocumentation(p.Description));

                @interface = @interface.AddMembers(property) as T;
            }
            return @interface;
        }

        private T GenerateImplementedInterfaces(I model, T @interface)
        {
            foreach(var i in model.ImplementedInterfaces)
            {
                @interface = _syntaxGenerator.AddInterfaceType(@interface, i.Syntax) as T;
            }
            return @interface;
        }

        private T GenerateTypeAttributes(I model, T type)
        {
            foreach (var a in model.Attributes)
            {
                type = type.AddAttributeLists(GenerateAttributeList(a as I)) as T;
            }
            return type;
        }

        private M GenerateMemberAttributes<M>(MethodModel model, M member) where M : MemberDeclarationSyntax
        {
            foreach (var a in model.Attributes)
            {
                member = member.AddAttributeLists(GenerateAttributeList(a as I)) as M;
            }
            return member;
        }

        private static AttributeListSyntax GenerateAttributeList(I a)
        {
            var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(a.Name));
            var attributeList = SyntaxFactory.AttributeList();
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
