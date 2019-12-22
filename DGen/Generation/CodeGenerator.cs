using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation
{
    public class CodeGenerator
    {
        private static readonly Regex AutoPropRegex = new Regex(@"\s*\{\s*get;\s*set;\s*}\s");
        private static readonly Regex AutoPropReadOnlyRegex = new Regex(@"\s*\{\s*get;\s*}\s");

        private readonly List<ICodeGenerator> _generators;
        private SyntaxGenerator _syntaxGenerator;
        private DirectoryInfo _basePath;

        public CodeGenerator(IEnumerable<ICodeGenerator> generators)
        {
            _generators = generators.ToList();
        }

        public async Task Generate(MetaModel model, string path)
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

                    foreach (var layer in _generators.GroupBy(g => g.Layer))
                    {
                        var layerDirectory = serviceDirectory.CreateSubdirectory(layer.Key);

                        await GenerateModule(layerDirectory, service, layer.ToList());
                    }
                }
            }    
        }

        private async Task GenerateModule(DirectoryInfo di, Module module, IEnumerable<ICodeGenerator> generators)
        {
            foreach (var generator in generators)
            {
                if(generator.GetFileNameForModule(module) != null)
                {
                    var subDirectory = generator.CreateSubdirectory(di);

                    // Generate module level code
                    using (var sw = File.CreateText(Path.Combine(subDirectory.FullName, generator.GetFileNameForModule(module))))
                    {
                        var node = generator.Generate(new CodeGenerationContext
                        {
                            Namespace = ResolveNamespace(subDirectory),
                            Module = module,
                            SyntaxGenerator = _syntaxGenerator
                        });
                        await WriteNodeToStream(sw, node);
                    }
                }
                else
                {
                    // Generate type level code
                    foreach (var type in generator.GetTypesFromModule(module))
                    {

                        var fileName = generator.GetFileName(type);

                        if(fileName != null)
                        {
                            var subDirectory = generator.CreateSubdirectory(di);

                            using (var sw = File.CreateText(Path.Combine(subDirectory.FullName, fileName)))
                            {
                                var node = generator.Generate(new CodeGenerationContext
                                {
                                    Namespace = ResolveNamespace(subDirectory),
                                    Module = module,
                                    Type = type,
                                    SyntaxGenerator = _syntaxGenerator
                                });
                                await WriteNodeToStream(sw, node);
                            }
                        }
                    }
                }    
            }

            module.Modules.ForEach(async m =>
            {
                await GenerateModule(di.CreateSubdirectory(m.Name), m, generators);
            });
        }

        private string ResolveNamespace(DirectoryInfo subDirectory)
        {
            var fullPath = subDirectory.FullName;
            var relativePath = fullPath.Substring(_basePath.FullName.Length+1);

            return relativePath.Replace("\\", ".");
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
