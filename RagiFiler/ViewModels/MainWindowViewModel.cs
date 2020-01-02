using System.Collections.ObjectModel;
using Prism.Mvvm;
using RagiFiler.IO;
using RagiFiler.ViewModels.Components;
using Reactive.Bindings;

namespace RagiFiler.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<TabItemViewModel> TabItems { get; } = new ObservableCollection<TabItemViewModel>();

        public ReactiveCommand LoadedCommand { get; } = new ReactiveCommand();

        public MainWindowViewModel()
        {
            LoadedCommand.Subscribe(OnLoaded);
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
    }

}
