using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators.Domain
{
    public class AggregateCodeGenerator : ICodeGenerator
    {
        public string Layer => "Domain";

        public SyntaxNode Generate(CodeGenerationContext context)
        {
            if (context.Type is Aggregate aggregate)
            {
                var builder = new ClassBuilder(context.SyntaxGenerator, context.Namespace, aggregate.Name);
                builder.AddBaseType("AggregateRoot");
                aggregate.GenerateProperties(builder, true);
                GenerateDomainEventHandlers(aggregate, builder, context.SyntaxGenerator);
                return builder.Build();
            }
            return null;
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

        public IEnumerable<BaseType> GetTypesFromModule(Module module)
        {
            return module.Aggregates;
        }    
    }
}
