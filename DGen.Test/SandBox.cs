﻿using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using DGen.Generation;
using DGen.Meta;
using DGen.StarUml;
using DGen.Generation.Generators.Domain;
using DGen.Generation.Generators.Infrastructure;
using DGen.Generation.Generators.Application;

namespace DGen.Test
{
    [TestFixture]
    public class SandBox
    {
        private readonly string _modelFileName = "HelloCustomer.mdj";
        private readonly JsonSerializerSettings _serializerSettings;

        public SandBox()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        [Test]
        public void CanReadStarUmlModel()
        {
            var model = new StarUmlReader().Read(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _modelFileName));
            Console.WriteLine(JsonConvert.SerializeObject(model, Formatting.Indented, _serializerSettings));
        }

        [Test]
        public void CanGenerateMetaModel()
        {
            var model = new StarUmlReader().Read(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _modelFileName));
            var metaModel = new MetaModelGenerator().Generate(model);
            Console.WriteLine(JsonConvert.SerializeObject(metaModel, Formatting.Indented, _serializerSettings));
        }

        [Test]
        public async Task CanGenerateCodeFromMetaModel()
        {
            var model = new StarUmlReader().Read(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _modelFileName));
            var metaModel = new MetaModelGenerator().Generate(model);

            var generators = new List<ICodeGenerator>
            {
                // Domain
                new AggregateCodeGenerator(),
                new DomainEventCodeGenerator(),
                new Generation.Generators.Domain.EntityCodeGenerator(),
                new EnumerationCodeGenerator(),
                new ValueCodeGenerator(),
                // Infrastructure
                new DbContextCodeGenerator(),
                new Generation.Generators.Infrastructure.EntityCodeGenerator(),
                // Application
                new ViewModelCodeGenerator(),
                new QueryCodeGenerator(),
                new QueryHandlerCodeGenerator()
            };

            await new CodeGenerator(generators).Generate(metaModel, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), metaModel.Name));
        }
    }
}
