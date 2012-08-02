using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WorkoutMusic
{
    public static class ExtensionMethods
    {
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.GetFiles();
            return files.Where(f => extensions.Contains(f.Extension));
        }
    }
}
