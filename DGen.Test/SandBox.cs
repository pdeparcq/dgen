using DGen.Test.StarUml;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;

namespace DGen.Test
{
    [TestFixture]
    public class SandBox
    {
        [Test]
        public void CanLoadStarUmlModel()
        {
            var json = File.ReadAllText(@"C:\Users\pdepa\OneDrive\Documenten\MyLunch.mdj");
            json = json.Replace("_id", "$id");

            var settings = new JsonSerializerSettings()
            {
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var model = JsonConvert.DeserializeObject<Element>(json, settings);

            Console.WriteLine(JsonConvert.SerializeObject(model, settings));
        }
    }
}
