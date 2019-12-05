using System.Threading.Tasks;

namespace DGen.Test.Generation
{
    public interface ICodeGenerator
    {
        string Name { get; }
        Task Generate(CodeGenerationContext context);
    }
}