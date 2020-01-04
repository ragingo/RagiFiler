using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Prism.Mvvm;
using RagiFiler.IO;
using RagiFiler.Media;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class TreeItemViewModel : BindableBase
    {
        public WeakReference<TreeItemViewModel> Parent { get; set; }
        public ObservableCollection<TreeItemViewModel> Children { get; } = new ObservableCollection<TreeItemViewModel>();
        public FileSystemInfo Item { get; set; }
        public bool IsDirectory { get { return Item is DirectoryInfo; } }
        public ReactiveCommand<object> SelectedItemChanged { get; } = new ReactiveCommand<object>();

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

        public int Level
        {
            get
            {
                if (Parent == null)
                {
                    return 0;
                }

                if (Parent.TryGetTarget(out var item))
                {
                    return item.Level + 1;
                }

                return 0;
            }
        }

        private TreeItemViewModel()
        {
            SelectedItemChanged.Subscribe(OnSelectedItemChanged);
        }

        public TreeItemViewModel(string path) : this()
        {
            Item = new DirectoryInfo(path);
        }

        public TreeItemViewModel(FileSystemInfo info) : this()
        {
            Item = info;
        }

        public async Task LoadSubDirectories()
        {
            if (Children.Count > 0)
            {
                return;
            }

            await foreach (var info in IOUtils.LoadFileSystemInfosAsync(Item.FullName).OfType<DirectoryInfo>())
            {
                var item = new TreeItemViewModel(info) { Parent = new WeakReference<TreeItemViewModel>(this) };
                Children.Add(item);
            }
        }

        private async void OnSelectedItemChanged(object value)
        {
            if (!(value is TreeItemViewModel item))
            {
                return;
            }

            if (!item.IsDirectory)
            {
                return;
            }

            try
            {
                await item.LoadSubDirectories().ConfigureAwait(true);
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
