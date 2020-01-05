using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace DGen.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup dependency injection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);  
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Get logger and generator from container
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var generator = serviceProvider.GetRequiredService<Generator>();

            logger.LogInformation($"Starting generation: ...");

            // Generate code
            Generate(generator).Wait();

            logger.LogInformation("Finished generation...");
        }

        private static async Task Generate(Generator generator)
        {
            await generator.Generate(Directory.GetCurrentDirectory(), Directory.GetCurrentDirectory());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Configure loggin
            services.AddLogging(configure => configure.AddConsole());
            services.AddDGen();
        }
    }
}
