using System;
using System.Collections.ObjectModel;
using System.Drawing;
using Prism.Mvvm;
using RagiFiler.IO;
using RagiFiler.Settings;
using RagiFiler.ViewModels.Components;
using Reactive.Bindings;

namespace RagiFiler.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public ReactiveCommand WindowLoadedCommand { get; } = new ReactiveCommand();
        public ReactiveCommand WindowClosingCommand { get; } = new ReactiveCommand();
        public ReactiveProperty<int> Width { get; } = new ReactiveProperty<int>(FilerSettings.Default.WindowSize.Width);
        public ReactiveProperty<int> Height { get; } = new ReactiveProperty<int>(FilerSettings.Default.WindowSize.Height);
        public ReactiveProperty<bool> ShowHiddenFiles { get; } = new ReactiveProperty<bool>(FilerSettings.Default.ShowHiddenFiles);
        public ReactiveProperty<bool> ShowSystemFiles { get; } = new ReactiveProperty<bool>(FilerSettings.Default.ShowSystemFiles);
        public ReactiveProperty<string> SearchMinSize { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> SearchMaxSize { get; } = new ReactiveProperty<string>();

        public ObservableCollection<TabItemViewModel> TabItems { get; } = new ObservableCollection<TabItemViewModel>();

        public MainWindowViewModel()
        {
            WindowLoadedCommand.Subscribe(OnLoaded);
            WindowClosingCommand.Subscribe(OnClosing);

            Width.Subscribe(OnWidthChanged);
            Height.Subscribe(OnHeightChanged);
            ShowHiddenFiles.Subscribe(OnShowHiddenFilesCheckChanged);
            ShowSystemFiles.Subscribe(OnShowSystemFilesCheckChanged);
        }

        private void OnWidthChanged(int value)
        {
            FilerSettings.Default.WindowSize = new Size(value, FilerSettings.Default.WindowSize.Height);
        }

        private void OnHeightChanged(int value)
        {
            FilerSettings.Default.WindowSize = new Size(FilerSettings.Default.WindowSize.Width, value);
        }

        private async void OnLoaded()
        {
            await foreach (var drive in IOUtils.LoadDrivesAsync())
            {
                var tab = new TabItemViewModel();
                TabItems.Add(tab);
                await tab.Load(drive.Name).ConfigureAwait(false);
            }
        }

        private void OnClosing()
        {
            FilerSettings.Default.Save();
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
