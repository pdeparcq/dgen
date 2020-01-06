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
    public class CSharpClassGenerator : CSharpInterfaceGenerator<ClassModel, ClassDeclarationSyntax>
    {
      
        public CSharpClassGenerator(SyntaxGenerator syntaxGenerator) : base(syntaxGenerator)
        {
        }

        protected override ClassDeclarationSyntax GenerateInterface(ClassModel model)
        {
            var @class = base.GenerateInterface(model);

            if (model.IsAbstract)
                @class = @class.AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword));

            @class = GenerateBaseType(model, @class);       
            @class = GenerateConstructors(model, @class);
            
            return @class;
        }

        protected override ClassDeclarationSyntax GenerateDeclaration(ClassModel model)
        {
            return _syntaxGenerator.ClassDeclaration(model.Name, accessibility: Accessibility.Public) as ClassDeclarationSyntax;
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

        private ClassDeclarationSyntax GenerateBaseType(ClassModel model, ClassDeclarationSyntax @class)
        {
            if (model.BaseType != null)
            {
                @class = _syntaxGenerator.AddBaseType(@class, model.BaseType.Syntax) as ClassDeclarationSyntax;
            }
            return @class;
        }
    }
}
