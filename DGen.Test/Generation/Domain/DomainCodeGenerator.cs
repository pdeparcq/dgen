using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Test.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DGen.Test.Generation.Domain
{
    public class DomainCodeGenerator : ICodeGenerator
    {
        public string Name => "Domain";

        public async Task Generate(CodeGenerationContext context)
        {
            foreach(var module in context.Service.Modules)
            {
                GenerateModule(context.Directory, module);
            }
        }

        private void GenerateModule(DirectoryInfo di, Module module)
        {
            di = di.CreateSubdirectory(module.Name);

            GenerateAggregates(module, di);

            module.Modules?.ForEach(m =>
            {
                GenerateModule(di, m);
            });
        }

        private void GenerateAggregates(Module module, DirectoryInfo di)
        {
            if(module.Aggregates != null && module.Aggregates.Any())
            {
                foreach(var aggregate in module.Aggregates)
                {
                    using (var sw = File.CreateText(Path.Combine(di.FullName, $"{aggregate.Name}.cs")))
                    {
                        var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(module.FullName));
                        sw.Write(ns.NormalizeWhitespace().ToFullString());
                    }
                }
            }
        }
    }
}
