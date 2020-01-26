using NUnit.Framework;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using DGen.Meta;
using DGen.StarUml;
using DGen.Generation;
using DGen.Generation.Generators.CSharp;

namespace DGen.Test
{
    [TestFixture]
    public class SandBox
    {    
        [Test]
        public async Task CanGenerateCodeForMyLunch()
        {
            await GenerateCodeFromModel("MyLunch.mdj");
        }

        private async Task GenerateCodeFromModel(string fileName)
        {
            var model = new StarUmlReader().Read(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName));
            var metaModel = new MetaModelGenerator().Generate(model);
            var application = new CodeModelGenerator().Generate(metaModel);
            await new CodeGenerator(new CSharpCodeGenerator()).Generate(application, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }
    }
}
