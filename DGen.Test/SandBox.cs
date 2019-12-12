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

namespace DGen.Test
{
    [TestFixture]
    public class SandBox
    {

        [Test]
        public void CanReadStarUmlModel()
        {
            var model = new StarUmlReader().Read(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MyLunch.mdj"));
            Console.WriteLine(JsonConvert.SerializeObject(model, Formatting.Indented));
        }

        [Test]
        public void CanGenerateMetaModel()
        {
            var model = new StarUmlReader().Read(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MyLunch.mdj"));
            var metaModel = new MetaModelGenerator().Generate(model);
            Console.WriteLine(JsonConvert.SerializeObject(metaModel, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
        }

        [Test]
        public async Task CanGenerateCodeFromMetaModel()
        {
            var model = new StarUmlReader().Read(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MyLunch.mdj"));
            var metaModel = new MetaModelGenerator().Generate(model);

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
