using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Domain.CodeGenerators
{
    public class ValueCodeGenerator : DomainCodeGeneratorBase
    {
        public ValueCodeGenerator(SyntaxGenerator generator) : base(generator)
        {
        }

        public override IEnumerable<BaseType> GetListFromModule(Module module)
        {
            return module.Values;
        }

        public override DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("ValueObjects");
        }

        public override async Task Generate(Module module, BaseType type, StreamWriter sw)
        {
            if (type is Value value)
            {
                var builder = new ClassBuilder(Generator, module.FullName, value.Name);
                value.GenerateProperties(builder);
                await sw.WriteAsync(builder.ToString());
            }
        }
    }
}
