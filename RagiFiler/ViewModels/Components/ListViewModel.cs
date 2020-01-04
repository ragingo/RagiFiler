using System;
using System.Collections.ObjectModel;
using System.IO;
using RagiFiler.IO;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class ListViewModel
    {
        public ReactiveProperty<string> Directory { get; } = new ReactiveProperty<string>();
        public ObservableCollection<ListItemViewModel> Entries { get; } = new ObservableCollection<ListItemViewModel>();

        public ListViewModel()
        {
            Directory.Subscribe(OnDirectoryChanged);
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

            Entries.Clear();

            try
            {
                await foreach (var entry in IOUtils.LoadFileSystemInfosAsync(value))
                {
                    Entries.Add(new ListItemViewModel(entry));
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
