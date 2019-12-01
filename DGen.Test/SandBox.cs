using DGen.Test.Meta;
using DGen.Test.StarUml;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace DGen.Test
{
    [TestFixture]
    public class SandBox
    {
        [Test]
        public void CanGenerateMetaModel()
        {
            var model = new StarUmlReader().Read(@"C:\Users\pdepa\OneDrive\Documenten\MyLunch.mdj");
            var metaModel = new MetaModelGenerator().Generate(model);
            Console.WriteLine(JsonConvert.SerializeObject(metaModel, Formatting.Indented));
        }     
    }
}
