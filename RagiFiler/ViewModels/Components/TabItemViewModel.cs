using System;
using System.Threading.Tasks;
using Prism.Mvvm;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class TabItemViewModel : BindableBase
    {
        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>();

        public TreeViewModel Tree { get; } = new TreeViewModel();

        public ListViewModel List { get; } = new ListViewModel();

        public TabItemViewModel()
        {
        }

        public async Task Load(string drive)
        {
            Tree.Root = new TreeItemViewModel(drive);
            await Tree.Root.LoadSubDirectories().ConfigureAwait(true);

            Tree.SelectedItemChanged.Subscribe(OnSelectedItemChanged);
        }

        private void OnSelectedItemChanged(object value)
        {
            if (!(value is TreeItemViewModel item))
            {
                return;
            }

            List.Directory.Value = item.Item.FullName;
        }
    }

}
