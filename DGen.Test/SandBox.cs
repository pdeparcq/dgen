using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using DGen.Generation;
using DGen.Generation.Application;
using DGen.Generation.Domain;
using DGen.Generation.Infrastructure;
using DGen.Generation.Presentation;
using DGen.Meta;
using DGen.StarUml;
using DGen.Meta.Generators;

namespace DGen.Test
{
    [TestFixture]
    public class SandBox
    {
        private readonly string _modelFileName = "HelloCustomer.mdj";
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly List<IMetaGenerator> _metaGenerators;

        public SandBox()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            _metaGenerators = new List<IMetaGenerator>() {
                new AggregateMetaGenerator(),
                new DomainEventMetaGenerator(),
                new EntityMetaGenerator(),
                new EnumerationMetaGenerator(),
                new QueryMetaGenerator(),
                new ValueMetaGenerator(),
                new ViewModelMetaGenerator()
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
            var metaModel = new MetaModelGenerator(_metaGenerators).Generate(model);
            Console.WriteLine(JsonConvert.SerializeObject(metaModel, Formatting.Indented, _serializerSettings));
        }

        [Test]
        public async Task CanGenerateCodeFromMetaModel()
        {
            var model = new StarUmlReader().Read(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _modelFileName));
            var metaModel = new MetaModelGenerator(_metaGenerators).Generate(model);

            var generators = new List<ICodeGenerator>
            {
                new DomainCodeGenerator(),
                new InfrastructureCodeGenerator(),
                new ApplicationCodeGenerator(),
                new PresentationCodeGenerator()
            };

            await new CodeGenerator(generators).Generate(metaModel, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "generated"));
        }
    }
}
