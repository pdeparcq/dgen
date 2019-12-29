using System;
using System.IO;
using System.Threading.Tasks;
using DGen.Generation.CodeModel;
using DGen.Generation.Generators;

namespace DGen.Generation
{
    public class CodeGenerator
    {
        private readonly ICodeGenerator _codeGenerator;
        private DirectoryInfo _basePath;
        
        public CodeGenerator(ICodeGenerator codeGenerator)
        {
            _codeGenerator = codeGenerator;
        }

        public async Task Generate(ApplicationModel model, string path)
        {
            _basePath = new DirectoryInfo(path);

            if (_basePath.Exists)
            {
                var applicationPath = _basePath.CreateSubdirectory(model.Name);

                RemoveDirectoryFiles(applicationPath);

                foreach (var service in model.Services)
                {
                    var serviceDirectory = applicationPath.CreateSubdirectory(service.Name);

                    foreach (var layer in service.Layers)
                    {
                        var layerDirectory = serviceDirectory.CreateSubdirectory(layer.Name);

                        await GenerateNamespace(layer, layerDirectory);
                        await _codeGenerator.GenerateProjectFile(layer, layerDirectory);
                    }
                }
            }
        }

        private async Task GenerateNamespace(NamespaceModel @namespace, DirectoryInfo di)
        {
            foreach (var type in @namespace.Types)
            {
                await GenerateCodeFile(type, di);
            }

            foreach (var ns in @namespace.Namespaces)
            {
                await GenerateNamespace(ns, di.CreateSubdirectory(ns.Name));
            }
        }

        private async Task GenerateCodeFile(TypeModel type, DirectoryInfo di)
        {
            if (type is ClassModel @class)
            {
                await _codeGenerator.GenerateClassFile(@class, di);
            }
            else if (type is EnumerationModel enumeration)
            {
                await _codeGenerator.GenerateEnumFile(enumeration, di);
            }
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
    }
}
