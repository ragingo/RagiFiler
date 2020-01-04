using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using RagiFiler.Native;

namespace RagiFiler.Media
{
    static class ImageUtils
    {
        private static readonly Dictionary<long, BitmapSource> _fileIconCache = new Dictionary<long, BitmapSource>();

        public static BitmapSource GetFileIcon(string path)
        {
            var shinfo = new SHFILEINFO();
            var result =
                NativeMethods.SHGetFileInfo(
                    path, 0, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo),
                    Constants.SHGFI_ICON | Constants.SHGFI_SMALLICON | Constants.SHGFI_LARGEICON
                );

            if (result == IntPtr.Zero)
            {
                return null;
            }

            if (shinfo.hIcon == IntPtr.Zero)
            {
                return null;
            }

            long key = shinfo.iIcon.ToInt64();

            if (!_fileIconCache.TryGetValue(key, out var bs))
            {
                bs = Imaging.CreateBitmapSourceFromHIcon(shinfo.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                bs.Freeze();
                _fileIconCache.Add(key, bs);
            }

            NativeMethods.DestroyIcon(shinfo.hIcon);

            return bs;
        }
    }
}
