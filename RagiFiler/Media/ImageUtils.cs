using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using RagiFiler.Native;

namespace RagiFiler.Media
{
    static class ImageUtils
    {
        public static BitmapSource GetFileIcon(string path)
        {
            var shinfo = new SHFILEINFO();
            var result =
                Functions.SHGetFileInfo(
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

            var bs = Imaging.CreateBitmapSourceFromHIcon(shinfo.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bs.Freeze();

            Functions.DestroyIcon(shinfo.hIcon);

            return bs;
        }
    }
}
