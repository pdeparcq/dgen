using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DGen.Generation.Infrastructure
{
    public class InfrastructureCodeGenerator : ICodeGenerator
    {
        public string Name => "Infrastructure";

        public async Task Generate(CodeGenerationContext context)
        {
            var syntaxGenerator = SyntaxGenerator.GetGenerator(context.Workspace, LanguageNames.CSharp);
            await GenerateModule(context.Service, context.Directory, syntaxGenerator);   
        }

        public async Task GenerateModule(Module module, DirectoryInfo di, SyntaxGenerator syntaxGenerator)
        {
            if(module.Aggregates != null && module.Aggregates.Any())
            {
                foreach (var aggregate in module.Aggregates)
                {
                    await GenerateAggregateEntity(aggregate, di.CreateSubdirectory("Entities"), syntaxGenerator);
                }
                await GenerateDbContext(module, di, syntaxGenerator);
            }

            module.Modules?.ForEach(async m =>
            {
                await GenerateModule(m, di.CreateSubdirectory(m.Name), syntaxGenerator);
            });
        }

        private async Task GenerateDbContext(Module module, DirectoryInfo di, SyntaxGenerator syntaxGenerator)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{module.Name}DbContext.cs")))
            {
                var builder = new ClassBuilder(syntaxGenerator, module.FullName, $"{module.Name}DbContext");
                await sw.WriteAsync(builder.ToString());
            }
        }

        private async Task GenerateAggregateEntity(Aggregate aggregate, DirectoryInfo di, SyntaxGenerator syntaxGenerator)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{aggregate.Name}.cs")))
            {
                var builder = new ClassBuilder(syntaxGenerator, aggregate.Module.FullName, aggregate.Name);
                await sw.WriteAsync(builder.ToString());
            }
        }
    }
}
