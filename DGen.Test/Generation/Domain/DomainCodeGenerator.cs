using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Test.Meta;

namespace DGen.Test.Generation.Domain
{
    public class DomainCodeGenerator : ICodeGenerator
    {
        public string Name => "Domain";

        public async Task Generate(CodeGenerationContext context)
        {
            foreach(var module in context.Service.Modules) 
            {
                var di = context.Directory.CreateSubdirectory(module.Name);

                GenerateAggregates(module.Aggregates, di);
            }
        }

        private void GenerateAggregates(List<Aggregate> aggregates, DirectoryInfo di)
        {
            if(aggregates != null && aggregates.Any())
            {
                foreach(var aggregate in aggregates)
                {
                    File.CreateText(Path.Combine(di.FullName, $"{aggregate.Name}.cs"));
                }
            }
        }
    }
}
