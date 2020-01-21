using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using RagiFiler.IO;
using RagiFiler.Models;
using RagiFiler.Native.Com;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class FileListViewViewModel
    {
        public ReactiveProperty<string> Directory { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<FileListViewItemViewModel> SelectedItem { get; } = new ReactiveProperty<FileListViewItemViewModel>();

        public ObservableCollection<FileListViewItemViewModel> Entries { get; } = new ObservableCollection<FileListViewItemViewModel>();

        public ReactiveCommand<object> SelectionChangedCommand { get; } = new ReactiveCommand<object>();
        public ReactiveCommand<object> MouseDoubleClick { get; } = new ReactiveCommand<object>();

        public FileListViewViewModel()
        {
            Directory.Subscribe(OnDirectoryChanged);
            SelectionChangedCommand.Subscribe(OnSelectionChanged);
            MouseDoubleClick.Subscribe(OnMouseDoubleClick);
        }

        private void OnSelectionChanged(object value)
        {
            if (value == null)
            {
                return;
            }

            if (!(value is FileListViewItemViewModel item))
            {
                return;
            }

            SelectedItem.Value = item;

            var model = new ContextMenuModel();
            var verbs = model.GetMenuItems(item.Item.FullName);
            foreach (var verb in verbs)
            {
                Debug.WriteLine(verb);
            }
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
                    Entries.Add(new FileListViewItemViewModel(entry));
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
