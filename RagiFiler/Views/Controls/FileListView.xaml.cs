﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RagiFiler.ViewModels.Components;

namespace RagiFiler.Views.Controls
{
    public partial class FileListView : UserControl
    {
        public FileListView()
        {
            InitializeComponent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ListView listView))
            {
                return;
            }

            if (!(listView.DataContext is FileListViewViewModel vm))
            {
                return;
            }

            if (e.AddedItems.Count == 0)
            {
                return;
            }

            if (!(e.AddedItems[0] is FileListViewItemViewModel itemVM))
            {
                return;
            }

            if (vm.SelectionChangedCommand.CanExecute())
            {
                vm.SelectionChangedCommand.Execute(itemVM);
            }
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!(sender is ListView listView))
            {
                return;
            }

            if (!(listView.DataContext is FileListViewViewModel vm))
            {
                return;
            }

            if (!(e.OriginalSource is FrameworkElement element))
            {
                return;
            }

            if (!(element.DataContext is FileListViewItemViewModel itemVM))
            {
                return;
            }

            if (vm.MouseDoubleClick.CanExecute())
            {
                vm.MouseDoubleClick.Execute(itemVM);
            }
        }
    }
}
