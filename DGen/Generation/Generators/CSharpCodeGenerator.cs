using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DGen.Generation.CodeModel;
using DGen.Generation.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators
{
    public class CSharpCodeGenerator : ICodeGenerator
    {
        private static readonly Regex AutoPropRegex = new Regex(@"\s*\{\s*get;\s*set;\s*}\s");
        private static readonly Regex AutoPropReadOnlyRegex = new Regex(@"\s*\{\s*get;\s*}\s");

        private SyntaxGenerator _syntaxGenerator;

        public CSharpCodeGenerator()
        {
            _syntaxGenerator = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);
        }
        public async Task GenerateClassFile(ClassModel @class, DirectoryInfo di)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{@class.Name}.cs")))
            {
                var builder = new ClassBuilder(_syntaxGenerator, @class.Namespace.FullName, @class.Name);
                await WriteNodeToStream(sw, builder.Build());
            }
        }

        public async Task GenerateEnumFile(EnumerationModel enumeration, DirectoryInfo di)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{enumeration.Name}.cs")))
            {
                var enumDeclaration = _syntaxGenerator.EnumDeclaration(enumeration.Name, Accessibility.Public) as EnumDeclarationSyntax;
                enumDeclaration = enumDeclaration.AddMembers(enumeration.Literals
                    .Select(l => _syntaxGenerator.EnumMember(l.Name) as EnumMemberDeclarationSyntax).ToArray());
                var ns = _syntaxGenerator.NamespaceDeclaration(enumeration.Namespace.FullName, enumDeclaration) as NamespaceDeclarationSyntax;
                await WriteNodeToStream(sw, ns);
            }
        }

        private async Task WriteNodeToStream(StreamWriter sw, SyntaxNode node)
        {
            await sw.WriteAsync(FormatAutoPropertiesOnOneLine(node.NormalizeWhitespace().ToFullString()));
        }

        private string FormatAutoPropertiesOnOneLine(string str)
        {
            str = AutoPropRegex.Replace(str, " { get; set; }");
            str = AutoPropReadOnlyRegex.Replace(str, " { get; }");
            return str;
        }
    }
}
