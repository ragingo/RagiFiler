using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RagiFiler.IO
{
    static class IOUtils
    {
        public static IEnumerable<FileSystemInfo> LoadFileSystemInfos(string path)
        {
            var fsi = new DirectoryInfo(path);
            var entries = fsi.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly);
            return entries;
        }

        public static IAsyncEnumerable<FileSystemInfo> LoadFileSystemInfosAsync(string path)
        {
            return LoadFileSystemInfos(path).ToAsyncEnumerable();
        }

        public static IAsyncEnumerable<DriveInfo> LoadDrivesAsync()
        {
            var drives = DriveInfo.GetDrives();
            return drives.ToAsyncEnumerable().Where(x => x.IsReady);
        }
    }
}
