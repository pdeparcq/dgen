using DGen.Generation;
using DGen.Generation.Generators.CSharp;
using DGen.Meta;
using DGen.StarUml;
using Microsoft.Extensions.DependencyInjection;

namespace DGen
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDGen(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddTransient<StarUmlReader>()
                .AddTransient<MetaModelGenerator>()
                .AddTransient<CodeModelGenerator>()
                .AddTransient<ICodeGenerator, CSharpCodeGenerator>()
                .AddTransient<CodeGenerator>()
                .AddTransient<Generator>();
        }
    }
}
