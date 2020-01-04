using System;
using Prism.Mvvm;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class TreeViewModel : BindableBase
    {
        public TreeItemViewModel Root { get; set; }

        public ReactiveProperty<TreeItemViewModel> SelectedItem { get; } = new ReactiveProperty<TreeItemViewModel>();

        public ReactiveCommand<object> SelectedItemChanged { get; } = new ReactiveCommand<object>();

        public TreeViewModel()
        {
            SelectedItemChanged.Subscribe(OnSelectedItemChanged);
        }

        private async void OnSelectedItemChanged(object value)
        {
            if (!(value is TreeItemViewModel item))
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
