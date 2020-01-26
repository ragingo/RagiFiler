using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RagiFiler.IO
{
    static class IOUtils
    {
        public static IEnumerable<FileSystemInfo> LoadFileSystemInfos(string path, string pattern = "*", bool recursive = false)
        {
            var fsi = new DirectoryInfo(path);
            var entries = fsi.EnumerateFileSystemInfos(pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            return entries;
        }

        public static IAsyncEnumerable<FileSystemInfo> LoadFileSystemInfosAsync(string path, string pattern = "*", bool recursive = false)
        {
            return LoadFileSystemInfos(path, pattern, recursive).ToAsyncEnumerable();
        }

        public static IAsyncEnumerable<DriveInfo> LoadDrivesAsync()
        {
            var drives = DriveInfo.GetDrives();
            return drives.ToAsyncEnumerable().Where(x => x.IsReady);
        }
    }
}
