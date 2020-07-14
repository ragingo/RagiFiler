using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Prism.Mvvm;
using RagiFiler.IO;
using RagiFiler.Media;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace RagiFiler.ViewModels.Components
{
    class DirectoryTreeViewItemViewModel : BindableBase
    {
        public WeakReference<DirectoryTreeViewItemViewModel> Parent { get; set; }
        public ObservableCollection<DirectoryTreeViewItemViewModel> Children { get; } = new ObservableCollection<DirectoryTreeViewItemViewModel>();
        public FileSystemInfo Item { get; set; }
        public bool IsDirectory { get { return Item is DirectoryInfo; } }
        public bool IsHiddenFile { get { return (Item.Attributes & FileAttributes.Hidden) > 0; } }
        public bool IsSystemFile { get { return (Item.Attributes & FileAttributes.System) > 0; } }
        public ReactiveProperty<bool> IsSelected { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsExpanded { get; } = new ReactiveProperty<bool>();
        public ReactiveCommand<bool> SelectionChanged { get; } = new ReactiveCommand<bool>();
        public ReactiveCommand<string> Renamed { get; } = new ReactiveCommand<string>();

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

        public DirectoryTreeViewItemViewModel(string path) : this(new DirectoryInfo(path))
        {
        }

        public DirectoryTreeViewItemViewModel(string path, WeakReference<DirectoryTreeViewItemViewModel> parent) : this(new DirectoryInfo(path), parent)
        {
        }

        public DirectoryTreeViewItemViewModel(FileSystemInfo info): this(info, null)
        {
            Item = info;
            IsSelected.Subscribe(OnSelected);
            Renamed.Subscribe(OnRenamed);
        }

        public DirectoryTreeViewItemViewModel(FileSystemInfo info, WeakReference<DirectoryTreeViewItemViewModel> parent)
        {
            Item = info;
            IsSelected.Subscribe(OnSelected);
            Renamed.Subscribe(OnRenamed);
            Parent = parent;
        }

        private void OnRenamed(string value)
        {
            Item = new DirectoryInfo(value);
            RaisePropertyChanged(nameof(Item));
            IsSelected.Subscribe(OnSelected);
        }

        private async void OnSelected(bool value)
        {
            if (value)
            {
                await LoadSubDirectories().ConfigureAwait(false);
            }

            IsExpanded.Value = value;

            SelectionChanged.Execute(value);
        }

        public async Task LoadSubDirectories()
        {
            if (Children.Count > 0)
            {
                return;
            }

            await foreach (var info in IOUtils.LoadFileSystemInfosAsync(Item.FullName).OfType<DirectoryInfo>())
            {
                var item = new DirectoryTreeViewItemViewModel(info, new WeakReference<DirectoryTreeViewItemViewModel>(this));
                Children.Add(item);
            }
        }
    }
}
