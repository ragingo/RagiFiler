using System;
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
            DirectoryTree.SelectedItemChanged.Subscribe(OnSelectedItemChanged);
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
    }

}
