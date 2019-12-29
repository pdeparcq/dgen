using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DGen.Generation.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension));
        }
    }
}
