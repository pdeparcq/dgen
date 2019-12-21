using DGen.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DGen.Generation
{
    public class CodeGenerationContext
    {
        public string Namespace{ get; set; }
        public Module Module { get; set; }
        public BaseType Type { get; set; }
        public SyntaxGenerator SyntaxGenerator { get; set; }
    }

    public interface ICodeGenerator
    {
        string Layer { get; }
        string Namespace { get; }
        IEnumerable<BaseType> GetTypesFromModule(Module module);
        DirectoryInfo CreateSubdirectory(DirectoryInfo di);
        string GetFileNameForModule(Module module);
        string GetFileName(BaseType type);
        SyntaxNode Generate(CodeGenerationContext context);
    }
}