using System.Threading.Tasks;

namespace DGen.Test.Generation.Application
{
    public class ApplicationCodeGenerator : ICodeGenerator
    {
        public string Name => "Application";

        public async Task Generate(CodeGenerationContext context)
        {
        }
    }
}
