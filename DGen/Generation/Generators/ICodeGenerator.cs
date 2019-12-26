using System.IO;
using System.Threading.Tasks;
using DGen.Generation.CodeModel;

namespace DGen.Generation.Generators
{
    public interface ICodeGenerator
    {
        Task GenerateClassFile(ClassModel @class, DirectoryInfo di);
        Task GenerateEnumFile(EnumerationModel enumeration, DirectoryInfo di);
    }
}