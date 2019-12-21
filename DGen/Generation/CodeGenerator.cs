using System;
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
        private Dictionary<object, string> _namespaces;

        public CodeGenerator(IEnumerable<ICodeGenerator> generators)
        {
            _generators = generators.ToList();
        }

        public async Task Generate(MetaModel model, string path)
        {
            _namespaces = new Dictionary<object, string>();

            var di = CreateDirectoryIfNotExists(path);
            var syntaxGenerator = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);

            GenerateServiceNamespaces(model);
            await GenerateServiceModules(model, di, syntaxGenerator);
        }

        private void GenerateServiceNamespaces(MetaModel model)
        {
            foreach (var service in model.Services)
            {
                foreach (var layer in _generators.GroupBy(g => g.Layer))
                {
                    GenerateNamespaces($"{model.Name}.{service.Name}.{layer.Key}", service, layer.ToList());
                }
            }
        }

        private void GenerateNamespaces(string baseNamespace, Module module, List<ICodeGenerator> generators)
        {
            RegisterNamespace(module, baseNamespace);

            foreach (var generator in generators)
            {
                foreach (var type in generator.GetTypesFromModule(module))
                {
                    if (generator.Namespace != null)
                        RegisterNamespace(type, $"{baseNamespace}.{generator.Namespace}");
                    else
                        RegisterNamespace(type, baseNamespace);
                }
            }

            module.Modules.ForEach(m =>
            {
                GenerateNamespaces($"{baseNamespace}.{m.Name}", m, generators);
            });
        }

        private async Task GenerateServiceModules(MetaModel model, DirectoryInfo di, SyntaxGenerator syntaxGenerator)
        {
            foreach (var service in model.Services)
            {
                var serviceDirectory = di.CreateSubdirectory(service.Name);

                foreach (var layer in _generators.GroupBy(g => g.Layer))
                {
                    var layerDirectory = serviceDirectory.CreateSubdirectory(layer.Key);

                    await GenerateModule(layerDirectory, service, layer.ToList(), syntaxGenerator);
                }
            }
        }

        private async Task GenerateModule(DirectoryInfo di, Module module, IEnumerable<ICodeGenerator> generators, SyntaxGenerator syntaxGenerator)
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
                            Namespace = ResolveNamespace(module),
                            Module = module,
                            SyntaxGenerator = syntaxGenerator
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
                                    Namespace = ResolveNamespace(type),
                                    Module = module,
                                    Type = type,
                                    SyntaxGenerator = syntaxGenerator
                                });
                                await WriteNodeToStream(sw, node);
                            }
                        }
                    }
                }    
            }

            module.Modules.ForEach(async m =>
            {
                await GenerateModule(di.CreateSubdirectory(m.Name), m, generators, syntaxGenerator);
            });
        }

        private async Task WriteNodeToStream(StreamWriter sw, SyntaxNode node)
        {
            await sw.WriteAsync(FormatAutoPropertiesOnOneLine(node.NormalizeWhitespace().ToFullString()));
        }

        private static DirectoryInfo CreateDirectoryIfNotExists(string path)
        {
            var di = new DirectoryInfo(path);
            if (di.Exists)
            {
                RemoveDirectory(di);
            }
            else
            {
                di.Create();
            }
            return di;
        }

        public void RegisterNamespace(object o, string @namespace)
        {
            _namespaces[o] = @namespace;
        }

        public string ResolveNamespace(object o)
        {
            return _namespaces.ContainsKey(o) ? _namespaces[o] : null;
        }

        private static void RemoveDirectory(DirectoryInfo di)
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
