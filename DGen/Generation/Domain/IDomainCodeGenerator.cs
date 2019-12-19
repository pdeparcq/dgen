using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Meta;

namespace DGen.Generation.Domain
{
    public interface IDomainCodeGenerator
    {
        IEnumerable<BaseType> GetListFromModule(Module module);
        DirectoryInfo CreateSubdirectory(DirectoryInfo di);
        string GetFileName(BaseType type);
        Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw);
    }
}