using System.IO;
using DGen.Test.Meta;
using Microsoft.CodeAnalysis;

namespace DGen.Test.Generation
{
    public class CodeGenerationContext
    {
        public Workspace Workspace { get; set; }
        public Service Service { get; set; }
        public DirectoryInfo Directory { get; set; }
    }
}
