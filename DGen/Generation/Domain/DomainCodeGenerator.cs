using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Generation.Domain.CodeGenerators;
using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Domain
{
    public class DomainCodeGenerator : ICodeGenerator
    {
        public string Name => "Domain";

        public async Task Generate(CodeGenerationContext context)
        {
            var syntaxGenerator = SyntaxGenerator.GetGenerator(context.Workspace, LanguageNames.CSharp);
            var generators = new List<IDomainCodeGenerator>()
            {
                new AggregateCodeGenerator(syntaxGenerator),
                new EntityCodeGenerator(syntaxGenerator),
                new DomainEventCodeGenerator(syntaxGenerator),
                new ValueCodeGenerator(syntaxGenerator),
                new EnumerationCodeGenerator(syntaxGenerator)
            };
            await GenerateModule(context.Namespace, context.Service, context.Directory, generators);
        }

        private async Task GenerateModule(string @namespace, Module module, DirectoryInfo di, List<IDomainCodeGenerator> generators)
        {

            foreach (var generator in generators)
            {
                foreach (var type in generator.GetListFromModule(module))
                {
                    var subDirectory = generator.CreateSubdirectory(di);
                    using (var sw = File.CreateText(Path.Combine(subDirectory.FullName, generator.GetFileName(type))))
                    {
                        await generator.Generate(@namespace, module, type, sw);
                    }
                }
            }

            module.Modules?.ForEach(async m =>
            {
                await GenerateModule($"{@namespace}.{m.Name}", m, di.CreateSubdirectory(m.Name), generators);
            });
        }
    }
}
