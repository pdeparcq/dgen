using System.Collections.Generic;
using System.IO;
using System.Linq;
using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DGen.Generation.Generators.Domain
{
    public class EnumerationCodeGenerator : ICodeGenerator
    {
        public string Layer => "Domain";

        public string Namespace => "Enumerations";

        public IEnumerable<BaseType> GetTypesFromModule(Module module)
        {
            return module.Enumerations;
        }

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("Enumerations");
        }

        public string GetFileNameForModule(Module module)
        {
            return null;
        }

        public string GetFileName(BaseType type)
        {
            return $"{type.Name}.cs";
        }

        public SyntaxNode Generate(CodeGenerationContext context)
        {
            if (context.Type is Enumeration enumeration)
            {
                var enumDeclaration = context.SyntaxGenerator.EnumDeclaration(enumeration.Name, Accessibility.Public) as EnumDeclarationSyntax;
                enumDeclaration = enumDeclaration.AddMembers(enumeration.Literals
                    .Select(l => context.SyntaxGenerator.EnumMember(l) as EnumMemberDeclarationSyntax).ToArray());
                return context.SyntaxGenerator.NamespaceDeclaration(context.Namespace, enumDeclaration) as NamespaceDeclarationSyntax;
            }
            return null;
        }
    }
}
