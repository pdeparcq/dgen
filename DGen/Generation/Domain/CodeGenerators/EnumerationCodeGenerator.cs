using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Domain.CodeGenerators
{
    public class EnumerationCodeGenerator : DomainCodeGeneratorBase
    {
        public EnumerationCodeGenerator(SyntaxGenerator generator) : base(generator)
        {
        }

        public override IEnumerable<BaseType> GetListFromModule(Module module)
        {
            return module.Enumerations;
        }

        public override DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("Enumerations");
        }

        public override async Task Generate(Module module, BaseType type, StreamWriter sw)
        {
            if (type is Enumeration enumeration)
            {
                var enumDeclaration = Generator.EnumDeclaration(enumeration.Name, Accessibility.Public) as EnumDeclarationSyntax;
                enumDeclaration = enumDeclaration.AddMembers(enumeration.Literals
                    .Select(l => Generator.EnumMember(l) as EnumMemberDeclarationSyntax).ToArray());
                var namespaceDeclaration = Generator.NamespaceDeclaration(module.FullName, enumDeclaration) as NamespaceDeclarationSyntax;

                await sw.WriteAsync(namespaceDeclaration.NormalizeWhitespace().ToFullString());
            }
            
        }
    }
}
