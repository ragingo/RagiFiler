using System;
using System.Windows;
using System.Windows.Threading;
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
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            string msg = e.Exception.Message + Environment.NewLine + e.Exception.StackTrace;
            MessageBox.Show(msg, "エラー発生！", MessageBoxButton.OK, MessageBoxImage.Error);
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
