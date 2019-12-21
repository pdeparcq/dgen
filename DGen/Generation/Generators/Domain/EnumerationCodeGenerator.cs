using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators.Domain
{
    public class EnumerationCodeGenerator : ICodeGenerator
    {
        public string Layer => "Domain";

        
        public IEnumerable<BaseType> GetListFromModule(Module module)
        {
            return module.Enumerations;
        }

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("Enumerations");
        }

        public async Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw, SyntaxGenerator syntaxGenerator)
        {
            if (type is Enumeration enumeration)
            {
                var enumDeclaration = syntaxGenerator.EnumDeclaration(enumeration.Name, Accessibility.Public) as EnumDeclarationSyntax;
                enumDeclaration = enumDeclaration.AddMembers(enumeration.Literals
                    .Select(l => syntaxGenerator.EnumMember(l) as EnumMemberDeclarationSyntax).ToArray());
                var namespaceDeclaration = syntaxGenerator.NamespaceDeclaration(@namespace, enumDeclaration) as NamespaceDeclarationSyntax;

                await sw.WriteAsync(namespaceDeclaration.NormalizeWhitespace().ToFullString());
            }
            
        }

        public string GetFileNameForModule(Module module)
        {
            return null;
        }

        public string GetFileName(BaseType type)
        {
            return $"{type.Name}.cs";
        }
    }
}
