using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DGen.Test
{
    [TestFixture]
    public class GeneratorTests
    {
        [Test]
        public async Task CanGenerateCodeFromModels()
        {
            var generator = new ServiceCollection()
                .AddLogging(configure => configure.AddConsole())
                .AddDGen()
                .BuildServiceProvider()
                .GetRequiredService<Generator>();

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            await generator.Generate(path, path);
        }
    }
}
