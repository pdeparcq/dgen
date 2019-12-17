using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Domain.CodeGenerators
{
    public class AggregateCodeGenerator : DomainCodeGeneratorBase
    {
        public AggregateCodeGenerator(SyntaxGenerator generator) : base(generator)
        {
        }

        public override IEnumerable<BaseType> GetListFromModule(Module module)
        {
            return module.Aggregates;
        }

        public override async Task Generate(Module module, BaseType type, StreamWriter sw)
        {
            if (type is Aggregate aggregate)
            {
                var builder = new ClassBuilder(Generator, module.FullName, aggregate.Name);
                builder.AddBaseType("AggregateRoot");
                aggregate.GenerateProperties(builder);
                GenerateDomainEventHandlers(aggregate, builder);
                await sw.WriteAsync(builder.ToString());
            }
        }

        private void GenerateDomainEventHandlers(Aggregate aggregate, ClassBuilder builder)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                var method = Generator.MethodDeclaration("Handle") as MethodDeclarationSyntax;
                method = method.AddParameterListParameters(Generator.ParameterDeclaration("e", SyntaxFactory.ParseTypeName(domainEvent.Name)) as ParameterSyntax);
                foreach (var property in domainEvent.Properties)
                {
                    if (aggregate.Properties.Any(p => p.Equals(property)))
                    {
                        //TODO: add body statements
                    }
                }
                builder.AddMethod(method);
            }
        }
    }
}
