using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Domain.CodeGenerators
{
    public class EntityCodeGenerator : DomainCodeGeneratorBase
    {
        public EntityCodeGenerator(SyntaxGenerator generator) : base(generator)
        {
        }

        public override IEnumerable<BaseType> GetListFromModule(Module module)
        {
            return module.Entities;
        }

        public override DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("Entities");
        }

        public override async Task Generate(Module module, BaseType type, StreamWriter sw)
        {
            if (type is Entity entity)
            {
                var builder = new ClassBuilder(Generator, module.FullName, entity.Name);
                builder.AddBaseType("Entity");
                entity.GenerateProperties(builder);
                await sw.WriteAsync(builder.ToString());
            }
        }
    }
}
