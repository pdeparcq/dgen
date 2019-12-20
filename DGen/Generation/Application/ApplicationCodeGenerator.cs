using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DGen.Generation.Application
{
    public class ApplicationCodeGenerator : ICodeGenerator
    {
        public string Name => "Application";

        public async Task Generate(CodeGenerationContext context)
        {
            var syntaxGenerator = SyntaxGenerator.GetGenerator(context.Workspace, LanguageNames.CSharp);
            await GenerateModule(context.Namespace, context.Service, context.Directory, syntaxGenerator);
        }

        public async Task GenerateModule(string @namespace, Module module, DirectoryInfo di, SyntaxGenerator syntaxGenerator)
        {
            if (module.Queries != null && module.Queries.Any())
            {
                foreach (var query in module.Queries)
                {
                    await GenerateQuery(@namespace, query, di.CreateSubdirectory("Queries"), syntaxGenerator);
                }
            }

            module.Modules?.ForEach(async m =>
            {
                await GenerateModule($"{@namespace}.{m.Name}", m, di.CreateSubdirectory(m.Name), syntaxGenerator);
            });
        }

        private async Task GenerateQuery(string @namespace, Query query, DirectoryInfo di, SyntaxGenerator syntaxGenerator)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{query.Name}.cs")))
            {
                var builder = new ClassBuilder(syntaxGenerator, @namespace, query.Name);
                query.GenerateProperties(builder);
                await sw.WriteAsync(builder.ToString());
            }
        }
    }
}
