using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using DGen.Meta;
using DGen.StarUml;
using DGen.Generation.Generators;
using DGen.Generation;

namespace DGen.Test
{
    [TestFixture]
    public class SandBox
    {
        private readonly string _modelFileName = "MyLunch.mdj";
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
            //Console.WriteLine(JsonConvert.SerializeObject(metaModel, Formatting.Indented, _serializerSettings));
        }

        [Test]
        public void CanGenerateApplicationModel()
        {
            var model = new StarUmlReader().Read(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _modelFileName));
            var metaModel = new MetaModelGenerator().Generate(model);
            var application = new CodeModelGenerator().Generate(metaModel);
            //Console.WriteLine(JsonConvert.SerializeObject(metaModel, Formatting.Indented, _serializerSettings));
        }

        [Test]
        public async Task CanGenerateCodeFromMetaModel()
        {
            var model = new StarUmlReader().Read(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _modelFileName));
            var metaModel = new MetaModelGenerator().Generate(model);
            var application = new CodeModelGenerator().Generate(metaModel);
            await new CodeGenerator(new CSharpCodeGenerator()).Generate(application, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }
    }
}
