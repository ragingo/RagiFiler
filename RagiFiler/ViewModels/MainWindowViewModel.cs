using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using RagiFiler.IO;
using RagiFiler.Settings;
using RagiFiler.ViewModels.Components;
using Reactive.Bindings;

namespace RagiFiler.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public ReactiveProperty<bool> ShowHiddenFiles { get; } = new ReactiveProperty<bool>(FilerSettings.Default.ShowHiddenFiles);

        public ObservableCollection<TabItemViewModel> TabItems { get; } = new ObservableCollection<TabItemViewModel>();

        public ReactiveCommand LoadedCommand { get; } = new ReactiveCommand();

        public MainWindowViewModel()
        {
            LoadedCommand.Subscribe(OnLoaded);

            ShowHiddenFiles.Subscribe(OnShowHiddenFilesCheckChanged);
        }

        private async void OnLoaded()
        {
            await foreach (var drive in IOUtils.LoadDrivesAsync())
            {
                var tab = new TabItemViewModel { Title = { Value = drive.Name } };
                TabItems.Add(tab);
                await tab.Load(drive.Name).ConfigureAwait(false);
            }
        }

        private void OnShowHiddenFilesCheckChanged(bool value)
        {
            FilerSettings.Default.ShowHiddenFiles = value;
            FilerSettings.Default.Save();
        }
    }
}
