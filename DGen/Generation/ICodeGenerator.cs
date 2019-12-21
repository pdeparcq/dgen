using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DGen.Generation
{
    public interface ICodeGenerator
    {
        string Layer { get; }
        IEnumerable<BaseType> GetTypesFromModule(Module module);
        DirectoryInfo CreateSubdirectory(DirectoryInfo di);
        string GetFileNameForModule(Module module);
        string GetFileName(BaseType type);
        Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw, SyntaxGenerator syntaxGenerator);
    }
}