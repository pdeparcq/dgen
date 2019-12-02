using DGen.Test.Meta;
using DGen.Test.StarUml;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using DGen.Test.Generation;

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
        public void CanGenerateCodeFromMetaModel()
        {
            var model = new StarUmlReader().Read(@"C:\dev\poc\HelloCustomer.mdj");
            var metaModel = new MetaModelGenerator().Generate(model);
            new CodeGenerator().Generate(metaModel, @"C:\dev\poc\generated");
        }
    }
}
