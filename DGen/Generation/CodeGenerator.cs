using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis;

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

            if(model.Services != null && model.Services.Any())
            {
                foreach(var service in model.Services)
                {
                    var serviceDirectory = di.CreateSubdirectory(service.Name);

                    foreach (var generator in _generators)
                    {
                        await generator.Generate(new CodeGenerationContext
                        {
                            Workspace = new AdhocWorkspace(),
                            Service = service,
                            Directory = serviceDirectory.CreateSubdirectory(generator.Name)
                        });
                    }
                }
            }
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
