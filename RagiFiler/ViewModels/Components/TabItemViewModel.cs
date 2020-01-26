using System;
using System.Linq;
using System.Threading.Tasks;
using Prism.Mvvm;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class TabItemViewModel : BindableBase
    {
        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>();
        public DirectoryTreeViewViewModel DirectoryTree { get; } = new DirectoryTreeViewViewModel();
        public FileListViewViewModel FileList { get; } = new FileListViewViewModel();

        public TabItemViewModel()
        {
            DirectoryTree.SelectedItem.Subscribe(OnSelectedItemChanged);
            FileList.Directory.Subscribe(OnFileListDirectoryChanged);
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
    }

}
