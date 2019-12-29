using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using DGen.Generation.CodeModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators.CSharp
{
    public class CSharpCodeGenerator : ICodeGenerator
    {
        private readonly SyntaxGenerator _syntaxGenerator;
        private readonly CSharpClassGenerator _classGenerator;

        public CSharpCodeGenerator()
        {
            _syntaxGenerator = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);
            _classGenerator = new CSharpClassGenerator(_syntaxGenerator);
        }

        public async Task GenerateClassFile(ClassModel @class, DirectoryInfo di)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{@class.Name}.cs")))
            {
                await sw.WriteAsync(_classGenerator.Generate(@class));
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
                await sw.WriteAsync(ns.NormalizeWhitespace().ToFullString());
            }
        }

        public async Task GenerateProjectFile(NamespaceModel layer, DirectoryInfo di)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{layer.Name}.csproj")))
            {
                var project = new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                    new XElement("PropertyGroup", new XElement("TargetFramework", "netcoreapp3.0")),
                    new XElement("ItemGroup", 
                        new XElement("PackageReference", new XAttribute("Include", "Kledex"), new XAttribute("Version", "2.3.0")),
                        new XElement("PackageReference", new XAttribute("Include", "Newtonsoft.Json"), new XAttribute("Version", "12.0.3"))
                        )
                    );
                await sw.WriteAsync(project.ToString());
            }
        }
    }
}
