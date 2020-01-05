using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using RagiFiler.IO;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class ListViewModel
    {
        public ReactiveProperty<string> Directory { get; } = new ReactiveProperty<string>();

        public ObservableCollection<ListItemViewModel> Entries { get; } = new ObservableCollection<ListItemViewModel>();

        public ReactiveCommand<object> MouseDoubleClick { get; } = new ReactiveCommand<object>();

        public ListViewModel()
        {
            Directory.Subscribe(OnDirectoryChanged);
            MouseDoubleClick.Subscribe(OnMouseDoubleClick);
        }

        private void OnMouseDoubleClick(object value)
        {
            if (value == null)
            {
                return;
            }

            if (!(value is ListItemViewModel item))
            {
                return;
            }

            var psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = item.Item.FullName;
            Process.Start(psi);
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
