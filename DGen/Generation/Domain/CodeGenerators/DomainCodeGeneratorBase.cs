using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Domain.CodeGenerators
{
    public abstract class DomainCodeGeneratorBase : IDomainCodeGenerator
    {
        protected SyntaxGenerator Generator { get; }

        protected DomainCodeGeneratorBase(SyntaxGenerator generator)
        {
            Generator = generator;
        }

        public abstract IEnumerable<BaseType> GetListFromModule(Module module);
        
        public virtual DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di;
        }

        public virtual string GetFileName(BaseType type)
        {
            return $"{type.Name}.cs";
        }

        public abstract Task Generate(Module module, BaseType type, StreamWriter sw);
    }
}