using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Domain.CodeGenerators
{
    public class DomainEventCodeGenerator : DomainCodeGeneratorBase
    {
        public DomainEventCodeGenerator(SyntaxGenerator generator) : base(generator)
        {
        }

        public override IEnumerable<BaseType> GetListFromModule(Module module)
        {
            return module.DomainEvents;
        }

        public override DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("DomainEvents");
        }

        public override async Task Generate(Module module, BaseType type, StreamWriter sw)
        {
            if (type is DomainEvent domainEvent)
            {
                var builder = new ClassBuilder(Generator, module.FullName, domainEvent.Name);
                builder.AddBaseType("DomainEvent");
                domainEvent.GenerateProperties(builder);
                await sw.WriteAsync(builder.ToString());
            }
        }
    }
}
