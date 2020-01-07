using System.IO;
using System.Windows.Media;
using Prism.Mvvm;
using RagiFiler.Media;

namespace RagiFiler.ViewModels.Components
{
    class FileListViewItemViewModel : BindableBase
    {
        public FileSystemInfo Item { get; private set; }
        public bool IsDirectory { get { return Item is DirectoryInfo; } }
        public bool IsHiddenFile { get { return (Item.Attributes & FileAttributes.Hidden) > 0; } }
        public bool IsSystemFile { get { return (Item.Attributes & FileAttributes.System) > 0; } }

        public long? FileSize
        {
            get
            {
                return (Item as FileInfo)?.Length;
            }
        }

        private ImageSource _icon;
        public ImageSource Icon
        {
            get
            {
                if (_icon == null)
                {
                    SetProperty(ref _icon, ImageUtils.GetFileIcon(Item.FullName), nameof(Icon));
                }
                return _icon;
            }
        }

        public FileListViewItemViewModel(FileSystemInfo info)
        {
            Item = info;
        }
    }
}
