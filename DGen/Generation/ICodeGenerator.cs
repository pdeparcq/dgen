using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Generation.CodeModel;

namespace DGen.Generation
{
    public interface ICodeGenerator
    {
        IEnumerable<string> CodeFileExtensions { get; }
        Task GenerateService(ServiceModel service, DirectoryInfo di);
        Task GenerateLayer(NamespaceModel layer, DirectoryInfo di);
        Task GenerateClassFile(ClassModel @class, DirectoryInfo di);
        Task GenerateEnumFile(EnumerationModel enumeration, DirectoryInfo di);
    }
}