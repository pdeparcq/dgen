using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Domain
{
    public class DomainCodeGenerator : ICodeGenerator
    {
        private SyntaxGenerator _generator;
        
        public string Name => "Domain";

        public async Task Generate(CodeGenerationContext context)
        {
            _generator = SyntaxGenerator.GetGenerator(context.Workspace, LanguageNames.CSharp);
            await GenerateModule(context.Service, context.Directory);
        }

        private async Task GenerateModule(Module module, DirectoryInfo di)
        {
            await GenerateAggregates(module, di);

            module.Modules?.ForEach(async m =>
            {
                await GenerateModule(m, di.CreateSubdirectory(m.Name));
            });
        }

        private async Task GenerateAggregates(Module module, DirectoryInfo di)
        {
            if(module.Aggregates != null && module.Aggregates.Any())
            {
                foreach (var aggregate in module.Aggregates)
                {
                    await GenerateAggregate(module, di, aggregate);
                }
            } 
        }

        private async Task GenerateAggregate(Module module, DirectoryInfo di, Aggregate aggregate)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{aggregate.Name}.cs")))
            {
                var builder = new ClassBuilder(_generator, module.FullName, aggregate.Name);
                builder.AddNamespaceImportDeclaration("System");
                builder.AddBaseType("AggregateRoot");
                aggregate.Properties?.ForEach(p => builder.AddAutoProperty(p.Name, p.Type));
                await sw.WriteAsync(builder.ToString());
            }
        }

    }
}
