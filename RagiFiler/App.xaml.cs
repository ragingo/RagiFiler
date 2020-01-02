using System.Windows;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using RagiFiler.ViewModels;
using RagiFiler.Views;

namespace RagiFiler
{
    public sealed partial class App : PrismApplication
    {
        public App()
        {
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
        }
    }
}
