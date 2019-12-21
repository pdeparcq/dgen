using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation
{
    public class CodeGenerator
    {

        private readonly List<ICodeGenerator> _generators;

        public CodeGenerator(IEnumerable<ICodeGenerator> generators)
        {
            _generators = generators.ToList();
        }

        public async Task Generate(MetaModel model, string path)
        {
            var di = CreateDirectoryIfNotExists(path);
            var syntaxGenerator = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);

            foreach (var service in model.Services)
            {
                var serviceDirectory = di.CreateSubdirectory(service.Name);

                foreach(var layer in _generators.GroupBy(g => g.Layer))
                {
                    var layerDirectory = serviceDirectory.CreateSubdirectory(layer.Key);

                    await GenerateModule($"{model.Name}.{service.Name}.{layer.Key}", layerDirectory, service, layer.ToList(), syntaxGenerator);
                }
            }
        }

        private async Task GenerateModule(string @namespace, DirectoryInfo di, Module module, IEnumerable<ICodeGenerator> generators, SyntaxGenerator syntaxGenerator)
        {
            foreach (var generator in _generators)
            {
                if(generator.GetFileNameForModule(module) != null)
                {
                    // Generate module level code
                    using (var sw = File.CreateText(Path.Combine(di.FullName, generator.GetFileNameForModule(module))))
                    {
                        await generator.Generate(@namespace, module, null, sw, syntaxGenerator);
                    }
                }
                else
                {
                    // Generate type level code
                    foreach (var type in generator.GetListFromModule(module))
                    {
                        var subDirectory = generator.CreateSubdirectory(di);

                        using (var sw = File.CreateText(Path.Combine(subDirectory.FullName, generator.GetFileName(type))))
                        {
                            await generator.Generate(@namespace, module, type, sw, syntaxGenerator);
                        }
                    }
                }    
            }

            module.Modules.ForEach(async m =>
            {
                await GenerateModule($"{@namespace}.{m.Name}", di.CreateSubdirectory(m.Name), m, generators, syntaxGenerator);
            });
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
    }
}
