using System;
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
        public FileListViewViewModel SearchResultFileList { get; } = new FileListViewViewModel();
        public ReactiveProperty<bool> IsSearchResultVisible { get; } = new ReactiveProperty<bool>();

        private object _selectedFileListItem;
        public object SelectedFileListItem
        {
            get { return _selectedFileListItem; }
            private set { SetProperty(ref _selectedFileListItem, value); }
        }


        public TabItemViewModel()
        {
            DirectoryTree.SelectedItem.Subscribe(OnSelectedItemChanged);
            FileList.Directory.Subscribe(OnFileListDirectoryChanged);
            Ribbon.SearchFileName.Subscribe(x => OnSearchValueChanged());
            Ribbon.SearchMinSize.Subscribe(x => OnSearchValueChanged());
            Ribbon.SearchMaxSize.Subscribe(x => OnSearchValueChanged());
            Ribbon.SearchDuplicateFile.Subscribe(x => OnSearchValueChanged());
            IsSearchResultVisible.Subscribe(OnSearchResultVisibleChanged);
            FileList.SelectedItem.Subscribe(OnFileListSelectedItemChanged);
            SearchResultFileList.SelectedItem.Subscribe(OnFileListSelectedItemChanged);
        }

        private void OnFileListSelectedItemChanged(FileListViewItemViewModel value)
        {
            SelectedFileListItem = value;
        }

        private void OnSearchResultVisibleChanged(bool value)
        {
            SelectedFileListItem = value ? SearchResultFileList.SelectedItem.Value : FileList.SelectedItem.Value;
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
                FileList.Directory.Value = null;
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

            if (DirectoryTree.ChangeSelectedItem.CanExecute())
            {
                DirectoryTree.ChangeSelectedItem.Execute(item);
            }
        }

        private async void OnSearchValueChanged()
        {
            SearchResultFileList.ClearEntries();

            if (FileList.Directory == null)
            {
                return;
            }

            string dir = FileList.Directory.Value;
            if (!Directory.Exists(dir))
            {
                return;
            }

            string name = Ribbon.SearchFileName.Value ?? "";
            if (!name.StartsWith("*"))
            {
                name = "*" + name;
            }

            if (!name.EndsWith("*"))
            {
                name += "*";
            }

            // TODO: キャンセル
            await foreach (var item in IOUtils.LoadFileSystemInfosAsync(dir, name, Ribbon.RecursiveSearch.Value).OfType<FileInfo>())
            {
                // min はどうでもいい
                _ = long.TryParse(Ribbon.SearchMinSize.Value?.Replace(",", ""), out long min);
                if (min > item.Length)
                {
                    continue;
                }

                // max は未入力で 0 になるから TryParse の結果を考慮
                if (long.TryParse(Ribbon.SearchMaxSize.Value?.Replace(",", ""), out long max) && max < item.Length)
                {
                    continue;
                }

                var itemVM = new FileListViewItemViewModel(item);
                if (Ribbon.SearchDuplicateFile.Value)
                {
                    itemVM.FileHash = await IOUtils.GetHashString(item).ConfigureAwait(true);
                    var dups = SearchResultFileList.RawEntries.Where(x => x.FileHash == itemVM.FileHash);
                    if (dups.Any())
                    {
                        itemVM.IsDuplicateFile = true;
                        foreach (var dup in dups)
                        {
                            dup.IsDuplicateFile = true;
                        }
                    }
                    SearchResultFileList.AddEntry(itemVM);
                }
                else
                {
                    SearchResultFileList.AddEntry(itemVM);
                }
            }

            IsSearchResultVisible.Value = true;
        }
    }
}
