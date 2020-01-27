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
    // TODO: リボン用プロパティを tab の vm に移動する
    class MainWindowViewModel : BindableBase
    {
        public ReactiveCommand WindowLoadedCommand { get; } = new ReactiveCommand();
        public ReactiveCommand WindowClosingCommand { get; } = new ReactiveCommand();
        public ReactiveProperty<int> Width { get; } = new ReactiveProperty<int>(FilerSettings.Default.WindowSize.Width);
        public ReactiveProperty<int> Height { get; } = new ReactiveProperty<int>(FilerSettings.Default.WindowSize.Height);

        public ReactiveProperty<TabItemViewModel> SelectedTabItem { get; } = new ReactiveProperty<TabItemViewModel>();
        public ObservableCollection<TabItemViewModel> TabItems { get; } = new ObservableCollection<TabItemViewModel>();

        public MainWindowViewModel()
        {
            WindowLoadedCommand.Subscribe(OnLoaded);
            WindowClosingCommand.Subscribe(OnClosing);
            Width.Subscribe(OnWidthChanged);
            Height.Subscribe(OnHeightChanged);
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
    }
}
