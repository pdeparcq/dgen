using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators
{
    public class AggregateCodeGenerator : ICodeGenerator
    {
        public string Layer => "Domain";

        public async Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw, SyntaxGenerator syntaxGenerator)
        {
            if (type is Aggregate aggregate)
            {
                var builder = new ClassBuilder(syntaxGenerator, @namespace, aggregate.Name);
                builder.AddBaseType("AggregateRoot");
                aggregate.GenerateProperties(builder);
                GenerateDomainEventHandlers(aggregate, builder, syntaxGenerator);
                await sw.WriteAsync(builder.ToString());
            }
        }

        private void GenerateDomainEventHandlers(Aggregate aggregate, ClassBuilder builder, SyntaxGenerator syntaxGenerator)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                var method = syntaxGenerator.MethodDeclaration("Handle") as MethodDeclarationSyntax;
                method = method.AddParameterListParameters(syntaxGenerator.ParameterDeclaration("e", SyntaxFactory.ParseTypeName(domainEvent.Name)) as ParameterSyntax);
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

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di;
        }

        public string GetFileNameForModule(Module module)
        {
            return null;
        }

        public string GetFileName(BaseType type)
        {
            return $"{type.Name}.cs";
        }

        public IEnumerable<BaseType> GetListFromModule(Module module)
        {
            return module.Aggregates;
        }
    }
}
