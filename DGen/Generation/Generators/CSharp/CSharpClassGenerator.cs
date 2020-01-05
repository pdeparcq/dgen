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

            if (model.IsAbstract)
                @class = @class.AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword));

            @class = GenerateBaseType(model, @class);
            @class = GenerateImplementedInterfaces(model, @class);
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
                var constructor = _syntaxGenerator.ConstructorDeclaration(c.Name, accessibility: c.Accessability) as ConstructorDeclarationSyntax;
                constructor = GenerateMethod(c, constructor);
                @class = @class.AddMembers(constructor);
            }
            return @class;
        }     

        private ClassDeclarationSyntax GenerateMethods(ClassModel model, ClassDeclarationSyntax @class)
        {
            foreach (var m in model.Methods)
            {
                var method = _syntaxGenerator.MethodDeclaration(m.Name, accessibility: m.Accessability, returnType: m.ReturnType?.Syntax) as MethodDeclarationSyntax;

                method = GenerateMethod(m, method);

                @class = @class.AddMembers(method);
            }
            return @class;
        }

        private T GenerateMethod<T>(MethodModel model, T method) where T : BaseMethodDeclarationSyntax
        {
            method = GenerateMemberAttributes(model, method);

            if (model.IsVirtual)
                method = method.AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword)) as T;

            if (model.IsAbstract)
                method = method.AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword)) as T;

            foreach (var p in model.Parameters)
            {
                var parameter = _syntaxGenerator.ParameterDeclaration(p.Name, p.Type.Syntax) as ParameterSyntax;
                method = method.AddParameterListParameters(parameter) as T;
            }

            if (!model.IsAbstract)
            {
                if(model.Body != null)
                    method = method.AddBodyStatements(model.Body.ToArray()) as T;
            }
            else
            {
                method = method.WithBody(null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)) as T;
            }

            return method;
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
                var property = SyntaxFactory.PropertyDeclaration(p.Type.Syntax, p.Name)
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
                @class = _syntaxGenerator.AddBaseType(@class, model.BaseType.Syntax) as ClassDeclarationSyntax;
            }
            return @class;
        }

        private ClassDeclarationSyntax GenerateImplementedInterfaces(ClassModel model, ClassDeclarationSyntax @class)
        {
            foreach(var @interface in model.ImplementedInterfaces)
            {
                @class = _syntaxGenerator.AddInterfaceType(@class, @interface.Syntax) as ClassDeclarationSyntax;
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
