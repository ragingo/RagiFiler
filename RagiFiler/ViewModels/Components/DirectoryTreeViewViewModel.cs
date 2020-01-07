using System;
using Prism.Mvvm;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class DirectoryTreeViewViewModel : BindableBase
    {
        public DirectoryTreeViewItemViewModel Root { get; set; }

        public ReactiveProperty<DirectoryTreeViewItemViewModel> SelectedItem { get; } = new ReactiveProperty<DirectoryTreeViewItemViewModel>();

        public ReactiveCommand<object> SelectedItemChanged { get; } = new ReactiveCommand<object>();

        public DirectoryTreeViewViewModel()
        {
            SelectedItemChanged.Subscribe(OnSelectedItemChanged);
        }

        private async void OnSelectedItemChanged(object value)
        {
            if (!(value is DirectoryTreeViewItemViewModel item))
            {
                return;
            }

            SelectedItem.Value = item;

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
