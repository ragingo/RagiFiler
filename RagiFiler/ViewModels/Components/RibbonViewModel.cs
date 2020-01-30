using System;
using RagiFiler.Settings;
using Reactive.Bindings;

namespace RagiFiler.ViewModels.Components
{
    class RibbonViewModel
    {
        public ReactiveProperty<bool> ShowHiddenFiles { get; } = new ReactiveProperty<bool>(FilerSettings.Default.ShowHiddenFiles);
        public ReactiveProperty<bool> ShowSystemFiles { get; } = new ReactiveProperty<bool>(FilerSettings.Default.ShowSystemFiles);
        public ReactiveProperty<bool> RecursiveSearch { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<string> SizeUnit { get; } = new ReactiveProperty<string>();
        public string[] SizeUnits { get; } = new string[] { "B", "KB", "MB", "GB", "TB" };
        public ReactiveProperty<string> SearchFileName { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> SearchMinSize { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> SearchMaxSize { get; } = new ReactiveProperty<string>();

        public RibbonViewModel()
        {
            ShowHiddenFiles.Subscribe(OnShowHiddenFilesCheckChanged);
            ShowSystemFiles.Subscribe(OnShowSystemFilesCheckChanged);
        }

        private void OnShowHiddenFilesCheckChanged(bool value)
        {
            FilerSettings.Default.ShowHiddenFiles = value;
            FilerSettings.Default.Save();
        }

        private void OnShowSystemFilesCheckChanged(bool value)
        {
            FilerSettings.Default.ShowSystemFiles = value;
            FilerSettings.Default.Save();
        }
    }
}
