using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Prism.Mvvm;
using RagiFiler.IO;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class TabItemViewModel : BindableBase
    {
        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>();
        public RibbonViewModel Ribbon { get; } = new RibbonViewModel();
        public DirectoryTreeViewViewModel DirectoryTree { get; } = new DirectoryTreeViewViewModel();
        public FileListViewViewModel FileList { get; } = new FileListViewViewModel();
        public ReactiveProperty<bool> IsSearchResultVisible { get; } = new ReactiveProperty<bool>();

        public TabItemViewModel()
        {
            DirectoryTree.SelectedItem.Subscribe(OnSelectedItemChanged);
            FileList.Directory.Subscribe(OnFileListDirectoryChanged);
            Ribbon.SearchFileName.Subscribe(OnSearchFileNameChanged);
        }

        public async Task Load(string drive)
        {
            Title.Value = drive;
            DirectoryTree.Root = new DirectoryTreeViewItemViewModel(drive);
            await DirectoryTree.Root.LoadSubDirectories().ConfigureAwait(true);
        }

        private void OnSelectedItemChanged(object value)
        {
            if (!(value is DirectoryTreeViewItemViewModel item))
            {
                return;
            }

            FileList.Directory.Value = item.Item.FullName;
        }

        private void OnFileListDirectoryChanged(string dir)
        {
            var treeViewItem = DirectoryTree.SelectedItem.Value;
            if (treeViewItem == null)
            {
                return;
            }

            var item = treeViewItem.Children.FirstOrDefault(x => x.Item.FullName == dir);
            if (item == null)
            {
                return;
            }

            if (DirectoryTree.SelectedItemChanged.CanExecute())
            {
                DirectoryTree.SelectedItemChanged.Execute(item);
            }
        }

        private async void OnSearchFileNameChanged(string value)
        {
            if (FileList.Directory == null)
            {
                return;
            }

            string dir = FileList.Directory.Value;
            if (!Directory.Exists(dir))
            {
                return;
            }

            if (!value.StartsWith("*"))
            {
                value = "*" + value;
            }

            if (!value.EndsWith("*"))
            {
                value += "*";
            }

            await foreach (var item in IOUtils.LoadFileSystemInfosAsync(dir, value, true).OfType<FileInfo>())
            {
                Debug.WriteLine(item.FullName);
            }

            IsSearchResultVisible.Value = true;
        }
    }
}
