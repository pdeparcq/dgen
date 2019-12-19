using System.IO;
using DGen.Meta;
using Microsoft.CodeAnalysis;

namespace DGen.Generation
{
    public class CodeGenerationContext
    {
        public string Namespace { get; set; }
        public Workspace Workspace { get; set; }
        public Service Service { get; set; }
        public DirectoryInfo Directory { get; set; }
    }
}
