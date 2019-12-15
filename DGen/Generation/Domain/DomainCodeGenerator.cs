using System;
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

                    foreach(var de in aggregate.DomainEvents)
                    {
                        await GenerateDomainEvent(module, di.CreateSubdirectory("DomainEvents"), de);
                    }
                }
            } 
        }

        private async Task GenerateDomainEvent(Module module, DirectoryInfo di, DomainEvent de)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{de.Name}.cs")))
            {
                var builder = new ClassBuilder(_generator, module.FullName, de.Name);
                builder.AddBaseType("DomainEvent");
                GenerateProperties(de, builder);
                await sw.WriteAsync(builder.ToString());
            }
        }

        private async Task GenerateAggregate(Module module, DirectoryInfo di, Aggregate aggregate)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{aggregate.Name}.cs")))
            {
                var builder = new ClassBuilder(_generator, module.FullName, aggregate.Name);
                builder.AddBaseType("AggregateRoot");
                GenerateProperties(aggregate, builder);
                await sw.WriteAsync(builder.ToString());
            }
        }

        private static void GenerateProperties(BaseType t, ClassBuilder builder)
        {
            t.Properties?.ForEach(p =>
            {
                if (p.Type.Type != null)
                {
                    builder.AddNamespaceImportDeclaration(p.Type.Type.Module.FullName);
                    builder.AddAutoProperty(p.Name, p.Type.Type.Name);
                }
                else
                {
                    builder.AddNamespaceImportDeclaration("System");
                    builder.AddAutoProperty(p.Name, p.Type.SystemType ?? "Undefined");
                }
            });
        }

    }
}
