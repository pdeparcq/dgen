using DGen.Generation;
using DGen.Generation.Extensions;
using DGen.Meta;
using DGen.StarUml;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DGen
{
    public class Generator
    {
        private readonly ILogger<Generator> _logger;
        private readonly StarUmlReader _starUmlReader;
        private readonly MetaModelGenerator _metaModelGenerator;
        private readonly CodeModelGenerator _codeModelGenerator;
        private readonly CodeGenerator _codeGenerator;

        public Generator(
            ILogger<Generator> logger,
            StarUmlReader starUmlReader,
            MetaModelGenerator metaModelGenerator,
            CodeModelGenerator codeModelGenerator,
            CodeGenerator codeGenerator)
        {
            _logger = logger;
            _starUmlReader = starUmlReader;
            _metaModelGenerator = metaModelGenerator;
            _codeModelGenerator = codeModelGenerator;
            _codeGenerator = codeGenerator;
        }

        public async Task Generate(string sourcePath, string destinationPath)
        {
            if (Directory.Exists(sourcePath) && Directory.Exists(destinationPath))
            {
                var di = new DirectoryInfo(sourcePath);

                foreach (var file in di.GetFilesByExtensions(".mdj"))
                {
                    try
                    {
                        await GenerateCodeFromModel(file, new DirectoryInfo(destinationPath));
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Failed to generate code for uml model: {file.FullName}");
                    }
                }
            }
            else
            {
                _logger.LogError($"Invalid source or destination path provided");
            }
        }

        private async Task GenerateCodeFromModel(FileInfo file, DirectoryInfo destination)
        {
            _logger.LogInformation($"Reading staruml model: {file.FullName}");
            var model = _starUmlReader.Read(file.FullName);
            _logger.LogInformation($"Generating meta model...");
            var metaModel = _metaModelGenerator.Generate(model);
            _logger.LogInformation($"Generating application model...");
            var application = _codeModelGenerator.Generate(metaModel);
            _logger.LogInformation($"Generating code to destination folder: {destination.FullName}");
            await _codeGenerator.Generate(application, destination.FullName);
        }
    }
}
