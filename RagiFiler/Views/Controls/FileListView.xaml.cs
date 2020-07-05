using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using RagiFiler.ViewModels.Components;

namespace RagiFiler.Views.Controls
{
    public partial class FileListView : UserControl
    {
        public FileListView()
        {
            InitializeComponent();

            listView.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown));
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            listView.UnselectAll();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(listView.DataContext is FileListViewViewModel vm))
            {
                return;
            }

            if (e.AddedItems.Count > 0)
            {
                if (!(e.AddedItems[0] is FileListViewItemViewModel itemVM))
                {
                    return;
                }

                if (vm.SelectionChangedCommand.CanExecute())
                {
                    vm.SelectionChangedCommand.Execute(itemVM);
                }
            }
            else if (e.RemovedItems.Count > 0)
            {
                if (vm.SelectionChangedCommand.CanExecute())
                {
                    vm.SelectionChangedCommand.Execute(null);
                }
            }
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
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

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
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

            if (vm.MouseRightClick.CanExecute())
            {
                vm.MouseRightClick.Execute(itemVM);
            }
        }

        private void OnGridViewColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            if (!(listView.DataContext is FileListViewViewModel vm))
            {
                return;
            }

            if (!(e.OriginalSource is GridViewColumnHeader gridViewColumnHeader))
            {
                return;
            }

            var binding = gridViewColumnHeader.Column.DisplayMemberBinding as Binding;
            if (binding == null)
            {
                return;
            }

            string member = binding.Path.Path;

            if (vm.ColumnHeaderClick.CanExecute())
            {
                vm.ColumnHeaderClick.Execute(member);
            }
        }
    }
}
