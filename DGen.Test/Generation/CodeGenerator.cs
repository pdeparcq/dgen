using System.IO;
using DGen.Test.Meta;

namespace DGen.Test.Generation
{
    public class CodeGenerator
    {
        public void Generate(MetaModel model, string path)
        {
            var di = CreateDirectoryIfExists(path);
        }

        private static DirectoryInfo CreateDirectoryIfExists(string path)
        {
            var di = new DirectoryInfo(path);
            if (di.Exists)
            {
                RemoveDirectory(di);
            }
            else
            {
                di.Create();
            }
            return di;
        }

        private static void RemoveDirectory(DirectoryInfo di)
        {
            foreach (var file in di.EnumerateFiles())
            {
                file.Delete();
            }

            foreach (var dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
