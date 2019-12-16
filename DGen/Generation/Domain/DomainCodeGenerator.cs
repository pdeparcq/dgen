﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            await GenerateValueObjects(module, di);
            await GenerateDomainEvents(module, di);
            await GenerateEntities(module, di);
            await GenerateAggregates(module, di);

            module.Modules?.ForEach(async m =>
            {
                await GenerateModule(m, di.CreateSubdirectory(m.Name));
            });
        }

        private async Task GenerateValueObjects(Module module, DirectoryInfo di)
        {
            if (module.Values != null && module.Values.Any())
            {
                di = di.CreateSubdirectory("ValueObjects");
                foreach (var value in module.Values)
                {
                    await GenerateValueObject(module, di, value);
                }
            }
        }

        private async Task GenerateDomainEvents(Module module, DirectoryInfo di)
        {
            if (module.DomainEvents != null && module.DomainEvents.Any())
            {
                di = di.CreateSubdirectory("DomainEvents");
                foreach (var domainEvent in module.DomainEvents)
                {
                    await GenerateDomainEvent(module, di, domainEvent);
                }
            }
        }

        private async Task GenerateEntities(Module module, DirectoryInfo di)
        {
            if (module.Entities != null && module.Entities.Any())
            {
                di = di.CreateSubdirectory("Entities");
                foreach (var entity in module.Entities)
                {
                    await GenerateEntity(module, di, entity);
                }
            }
        }
        private async Task GenerateAggregates(Module module, DirectoryInfo di)
        {
            if(module.Aggregates != null && module.Aggregates.Any())
            {
                foreach (var aggregate in module.Aggregates)
                {
                    await GenerateAggregate(module, di, aggregate);
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

        private async Task GenerateValueObject(Module module, DirectoryInfo di, Value value)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{value.Name}.cs")))
            {
                var builder = new ClassBuilder(_generator, module.FullName, value.Name);
                GenerateProperties(value, builder);
                await sw.WriteAsync(builder.ToString());
            }
        }

        private async Task GenerateEntity(Module module, DirectoryInfo di, Entity entity)
        {
            using (var sw = File.CreateText(Path.Combine(di.FullName, $"{entity.Name}.cs")))
            {
                var builder = new ClassBuilder(_generator, module.FullName, entity.Name);
                builder.AddBaseType("Entity");
                GenerateProperties(entity, builder);
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
                GenerateDomainEventHandlers(aggregate, builder);
                await sw.WriteAsync(builder.ToString());
            }
        }

        private void GenerateDomainEventHandlers(Aggregate aggregate, ClassBuilder builder)
        {
            foreach(var domainEvent in aggregate.DomainEvents)
            {
                var method = _generator.MethodDeclaration("Handle") as MethodDeclarationSyntax;
                method = method.AddParameterListParameters(_generator.ParameterDeclaration("e", SyntaxFactory.ParseTypeName(domainEvent.Name)) as ParameterSyntax);
                foreach(var property in domainEvent.Properties)
                {
                    if (aggregate.Properties.Any(p => p.Equals(property)))
                    {
                        //TODO: add body statements
                    }
                }
                builder.AddMethod(method);
            }
        }

        private static void GenerateProperties(BaseType t, ClassBuilder builder)
        {
            t.Properties?.ForEach(p =>
            {
                if (p.IsCollection)
                    builder.AddNamespaceImportDeclaration("System.Collections.Generic");

                if (p.Type.Type != null)
                {
                    builder.AddNamespaceImportDeclaration(p.Type.Type.Module.FullName);
                }
                else
                {
                    builder.AddNamespaceImportDeclaration("System");
                }

                builder.AddAutoProperty(p.Name, p.IsCollection ? $"List<{p.Type.Name}>" : p.Type.Name);
            });
        }

    }
}
