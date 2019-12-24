using System;
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
    public class CSharpCodeGenerator
    {
        private static readonly Regex AutoPropRegex = new Regex(@"\s*\{\s*get;\s*set;\s*}\s");
        private static readonly Regex AutoPropReadOnlyRegex = new Regex(@"\s*\{\s*get;\s*}\s");

        private SyntaxGenerator _syntaxGenerator;
        private DirectoryInfo _basePath;

        public async Task Generate(ApplicationModel model, string path)
        {
            _basePath = new DirectoryInfo(path);

            if (_basePath.Exists)
            {
                _syntaxGenerator = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);

                var applicationPath = _basePath.CreateSubdirectory(model.Name);

                RemoveDirectoryFiles(applicationPath);

                foreach (var service in model.Services)
                {
                    var serviceDirectory = applicationPath.CreateSubdirectory(service.Name);

                    foreach (var layer in service.Layers)
                    {
                        var layerDirectory = serviceDirectory.CreateSubdirectory(layer.Name);

                        await GenerateNamespace(layer, layerDirectory);
                    }
                }
            }
        }

        private async Task GenerateNamespace(NamespaceModel @namespace, DirectoryInfo di)
        {
            foreach(var type in @namespace.Types)
            {
                await GenerateCodeFile(type, di);
            }

            foreach(var ns in @namespace.Namespaces)
            {
                await GenerateNamespace(ns, di.CreateSubdirectory(ns.Name));
            }
        }

        private async Task GenerateCodeFile(TypeModel type, DirectoryInfo di)
        {
            if(type is ClassModel @class)
            {
                await GenerateClassFile(@class, di);
            }
            else if(type is EnumerationModel enumeration)
            {
                await GenerateEnumFile(enumeration, di);
            }

        }

        private async Task GenerateClassFile(ClassModel @class, DirectoryInfo di)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{@class.Name}.cs")))
            {
                var builder = new ClassBuilder(_syntaxGenerator, @class.Namespace.FullName, @class.Name);
                await WriteNodeToStream(sw, builder.Build());
            }
        }

        private async Task GenerateEnumFile(EnumerationModel enumeration, DirectoryInfo di)
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

        private static void RemoveDirectoryFiles(DirectoryInfo di)
        {
            foreach (var file in di.EnumerateFiles())
            {
                file.Delete();
            }

            foreach (var dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }

        private string FormatAutoPropertiesOnOneLine(string str)
        {
            str = AutoPropRegex.Replace(str, " { get; set; }");
            str = AutoPropReadOnlyRegex.Replace(str, " { get; }");
            return str;
        }
    }
}
