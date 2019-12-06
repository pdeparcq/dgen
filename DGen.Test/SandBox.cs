using DGen.Test.Meta;
using DGen.Test.StarUml;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DGen.Test.Generation;
using DGen.Test.Generation.Application;
using DGen.Test.Generation.Domain;
using DGen.Test.Generation.Infrastructure;
using DGen.Test.Generation.Presentation;

namespace DGen.Test
{
    [TestFixture]
    public class SandBox
    {
        [Test]
        public void CanReadStarUmlModel()
        {
            var model = new StarUmlReader().Read(@"C:\dev\poc\HelloCustomer.mdj");
            Console.WriteLine(JsonConvert.SerializeObject(model, Formatting.Indented));
        }

        [Test]
        public void CanGenerateMetaModel()
        {
            var model = new StarUmlReader().Read(@"C:\dev\poc\HelloCustomer.mdj");
            var metaModel = new MetaModelGenerator().Generate(model);
            Console.WriteLine(JsonConvert.SerializeObject(metaModel, Formatting.Indented));
        }

        [Test]
        public async Task CanGenerateCodeFromMetaModel()
        {
            var model = new StarUmlReader().Read(@"C:\dev\poc\MyLunch.mdj");
            var metaModel = new MetaModelGenerator().Generate(model);

            var generators = new List<ICodeGenerator>
            {
                new DomainCodeGenerator(),
                new InfrastructureCodeGenerator(),
                new ApplicationCodeGenerator(),
                new PresentationCodeGenerator()
            };

            await new CodeGenerator(generators).Generate(metaModel, @"C:\dev\poc\generated");
        }
    }
}
