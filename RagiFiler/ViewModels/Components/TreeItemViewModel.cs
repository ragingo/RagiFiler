using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private static readonly Dictionary<string, ImageSource> _iconCache = new Dictionary<string, ImageSource>();

        public WeakReference<TreeItemViewModel> Parent { get; set; }
        public ObservableCollection<TreeItemViewModel> Children { get; } = new ObservableCollection<TreeItemViewModel>();
        public FileSystemInfo Item { get; set; }
        public bool IsDirectory { get { return Item is DirectoryInfo; } }

        public ReactiveCommand<TreeItemViewModel> SelectedItemChanged { get; } = new ReactiveCommand<TreeItemViewModel>();

        private ImageSource _icon;
        public ImageSource Icon
        {
            get
            {
                if (_icon == null && Item != null)
                {
                    var img = ImageUtils.GetFileIcon(Item.FullName);
                    if (!_iconCache.ContainsKey(Item.FullName))
                    {
                        _iconCache[Item.FullName] = img;
                    }
                    _icon = _iconCache[Item.FullName];
                    RaisePropertyChanged(nameof(Icon));
                }
                return _icon;
            }
        }

        TreeItemViewModel()
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
            await foreach (var dir in IOUtils.LoadDirectoriesAsync(Item.FullName))
            {
                var item = new TreeItemViewModel(dir) { Parent = new WeakReference<TreeItemViewModel>(this) };
                Children.Add(item);
            }
        }

        private async void OnSelectedItemChanged(TreeItemViewModel item)
        {
            if (!item.IsDirectory)
            {
                return;
            }

            await item.LoadSubDirectories().ConfigureAwait(true);
        }
    }
}
