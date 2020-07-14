using System;
using System.IO;
using System.Windows.Threading;
using Prism.Mvvm;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class DirectoryTreeViewViewModel : BindableBase
    {
        public DirectoryTreeViewItemViewModel Root { get; set; }
        public ReactiveProperty<DirectoryTreeViewItemViewModel> SelectedItem { get; } = new ReactiveProperty<DirectoryTreeViewItemViewModel>();
        public ReactiveCommand<object> ChangeSelectedItem { get; } = new ReactiveCommand<object>();

        private FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();
        private readonly Dispatcher uiThreadDispatcher;

        public DirectoryTreeViewViewModel()
        {
            uiThreadDispatcher = Dispatcher.CurrentDispatcher;

            _fileSystemWatcher.IncludeSubdirectories = true;
            _fileSystemWatcher.Changed += OnFileChanged;
            _fileSystemWatcher.Created += OnFileChanged;
            _fileSystemWatcher.Deleted += OnFileChanged;
            _fileSystemWatcher.Renamed += OnFileRenamed;

            ChangeSelectedItem.Subscribe(OnSelectedItemChanged);
        }

        // ファイルが変更されたらリフレッシュ
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            _fileSystemWatcher.EnableRaisingEvents = false;

            var target = FindDirectoryItemViewModel(Root, e.FullPath);

            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                uiThreadDispatcher.Invoke(new Action(() => {
                    if (target?.Parent != null)
                    {
                        if (target.Parent.TryGetTarget(out var parent))
                        {
                            parent.Children.Remove(target);
                            SelectedItem.Value = null;
                            ChangeSelectedItem.Execute(parent);
                        }
                    }
                }));
            }
            else if (e.ChangeType == WatcherChangeTypes.Created)
            {
                uiThreadDispatcher.Invoke(new Action(() => {
                    var fi = new FileInfo(e.FullPath);
                    if (fi.Directory != null && Directory.Exists(fi.FullName))
                    {
                        var parent = FindDirectoryItemViewModel(Root, fi.Directory.FullName);
                        var child = new DirectoryTreeViewItemViewModel(e.FullPath, new WeakReference<DirectoryTreeViewItemViewModel>(parent));
                        parent.Children.Add(child);
                        SelectedItem.Value = null;
                        ChangeSelectedItem.Execute(parent);
                    }
                }));
            }
            else
            {
                uiThreadDispatcher.Invoke(new Action<DirectoryTreeViewItemViewModel>(OnSelectedItemChanged), target);
            }

            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            _fileSystemWatcher.EnableRaisingEvents = false;

            var target = FindDirectoryItemViewModel(Root, e.OldFullPath);
            target.Renamed.Execute(e.FullPath);

            uiThreadDispatcher.Invoke(new Action<DirectoryTreeViewItemViewModel>(OnSelectedItemChanged), target);

            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnSelectedItemChanged(object value)
        {
            if (!(value is DirectoryTreeViewItemViewModel item))
            {
                return;
            }

            SelectedItem.Value = item;
            SelectedItem.Value.IsSelected.Value = true;

            _fileSystemWatcher.Path = item.Item.FullName;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private DirectoryTreeViewItemViewModel FindDirectoryItemViewModel(DirectoryTreeViewItemViewModel itemVM, string path)
        {
            if (itemVM == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (itemVM.Item.FullName == path)
            {
                return itemVM;
            }

            foreach (var child in itemVM.Children)
            {
                var result = FindDirectoryItemViewModel(child, path);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
