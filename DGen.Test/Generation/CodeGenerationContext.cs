using System.IO;
using DGen.Test.Meta;

namespace DGen.Test.Generation
{
    public class CodeGenerationContext
    {
        public Service Service { get; set; }
        public DirectoryInfo Directory { get; set; }
    }
}
