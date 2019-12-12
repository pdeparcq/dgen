using System.Threading.Tasks;

namespace DGen.Generation
{
    public interface ICodeGenerator
    {
        string Name { get; }
        Task Generate(CodeGenerationContext context);
    }
}