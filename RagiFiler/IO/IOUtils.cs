using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;

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

        public static Task<string> GetHashString(string path)
        {
            return GetHashString(new FileInfo(path));
        }

        public static async Task<string> GetHashString(FileSystemInfo fileSystemInfo)
        {
            if (!(fileSystemInfo is FileInfo file))
            {
                return "";
            }

            if (file.Attributes.HasFlag(FileAttributes.Hidden | FileAttributes.System))
            {
                return "";
            }

            if (file.Length == 0)
            {
                return "";
            }

            long bufSize = file.Length > (long)1e6 ? (long)1e6 : file.Length;
            byte[] buf = new byte[bufSize];

            using (var fs = file.OpenRead())
            using (var md5 = MD5.Create())
            {
                int len = await fs.ReadAsync(buf).ConfigureAwait(false);
                byte[] bytes = md5.ComputeHash(buf, 0, len);
                var sb = new StringBuilder(len * 2);
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.AppendFormat("{0:X2}", bytes[i]);
                }
                return sb.ToString();
            }
        }
    }
}
