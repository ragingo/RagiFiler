using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using RagiFiler.IO;
using RagiFiler.Models;
using RagiFiler.Native.Com.Shell;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class FileListViewViewModel : IDisposable
    {
        public ReactiveProperty<string> Directory { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<FileListViewItemViewModel> SelectedItem { get; } = new ReactiveProperty<FileListViewItemViewModel>();

        private ObservableCollection<FileListViewItemViewModel> _rawEntries = new ObservableCollection<FileListViewItemViewModel>();
        public ReadOnlyCollection<FileListViewItemViewModel> RawEntries { get; private set; }
        public ICollectionView Entries { get; private set; }
        public ObservableCollection<FolderItemVerb> ContextMenuItems { get; } = new ObservableCollection<FolderItemVerb>();

        public ReactiveCommand<object> SelectionChangedCommand { get; } = new ReactiveCommand<object>();
        public ReactiveCommand<object> MouseDoubleClick { get; } = new ReactiveCommand<object>();
        public ReactiveCommand<object> MouseRightClick { get; } = new ReactiveCommand<object>();
        public ReactiveCommand<object> ContextMenuItemClick { get; } = new ReactiveCommand<object>();
        public ReactiveCommand<object> ColumnHeaderClick { get; } = new ReactiveCommand<object>();

        private FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();

        private readonly Dispatcher uiThreadDispatcher;

        public FileListViewViewModel()
        {
            uiThreadDispatcher = Dispatcher.CurrentDispatcher;

            RawEntries = new ReadOnlyCollection<FileListViewItemViewModel>(_rawEntries);
            Entries = CollectionViewSource.GetDefaultView(_rawEntries);

            Directory.Subscribe(OnDirectoryChanged);
            SelectionChangedCommand.Subscribe(OnSelectionChanged);
            MouseDoubleClick.Subscribe(OnMouseDoubleClick);
            MouseRightClick.Subscribe(OnMouseRightClick);
            ContextMenuItemClick.Subscribe(OnContextMenuItemClick);
            ColumnHeaderClick.Subscribe(OnColumnHeaderClick);

            _fileSystemWatcher.IncludeSubdirectories = true;
            _fileSystemWatcher.Changed += OnFileChanged;
            _fileSystemWatcher.Created += OnFileChanged;
            _fileSystemWatcher.Deleted += OnFileChanged;
            _fileSystemWatcher.Renamed += OnFileChanged;
        }

        // ファイルが変更されたらリフレッシュ
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            uiThreadDispatcher.Invoke(new Action<string>(OnDirectoryChanged), Directory.Value);
        }

        public void AddEntry(FileListViewItemViewModel item)
        {
            _rawEntries.Add(item);
        }

        public void ClearEntries()
        {
            _rawEntries.Clear();
        }

        private void OnSelectionChanged(object value)
        {
            if (value == null)
            {
                SelectedItem.Value = null;
                return;
            }

            if (!(value is FileListViewItemViewModel item))
            {
                return;
            }

            SelectedItem.Value = item;
        }

        private void OnMouseDoubleClick(object value)
        {
            if (value == null)
            {
                return;
            }

            if (!(value is FileListViewItemViewModel item))
            {
                return;
            }

            if (item.IsDirectory)
            {
                Directory.Value = item.Item.FullName;
                return;
            }

            try
            {
                var psi = new ProcessStartInfo();
                psi.UseShellExecute = true;
                psi.FileName = item.Item.FullName;
                Process.Start(psi);
            }
            catch (FileNotFoundException)
            {
                // TODO: 通知
            }
        }

        private void OnMouseRightClick(object value)
        {
            if (value == null)
            {
                return;
            }

            if (!(value is FileListViewItemViewModel item))
            {
                return;
            }

            foreach (var menuItem in ContextMenuItems)
            {
                menuItem.Dispose();
            }
            ContextMenuItems.Clear();

            var model = new ContextMenuModel();
            foreach (var menuItem in model.GetMenuItems(item.Item.FullName))
            {
                ContextMenuItems.Add(menuItem);
            }
        }

        private void OnContextMenuItemClick(object value)
        {
            if (!(value is FolderItemVerb verb))
            {
                return;
            }

            verb.DoIt();
        }

        private void OnColumnHeaderClick(object value)
        {
            if (!(value is string member))
            {
                return;
            }

            var direction = ListSortDirection.Descending;
            var lastSort = Entries.SortDescriptions.FirstOrDefault(x => x.PropertyName == member);
            if (lastSort != null)
            {
                direction = lastSort.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            Entries.SortDescriptions.Clear();
            Entries.SortDescriptions.Add(new SortDescription { PropertyName = member, Direction = direction });
        }

        private async void OnDirectoryChanged(string value)
        {
            if (value == null)
            {
                return;
            }

            var info = new DirectoryInfo(value);
            if (!info.Exists)
            {
                return;
            }

            _fileSystemWatcher.EnableRaisingEvents = false;

            _rawEntries.Clear();

            try
            {
                await foreach (var entry in IOUtils.LoadFileSystemInfosAsync(value))
                {
                    _rawEntries.Add(new FileListViewItemViewModel(entry));
                }
            }
            catch (UnauthorizedAccessException)
            {
            }

            _fileSystemWatcher.Path = value;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
            _fileSystemWatcher.Dispose();
        }
    }
}
