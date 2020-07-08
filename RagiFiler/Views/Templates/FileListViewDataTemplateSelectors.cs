using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using RagiFiler.ViewModels.Components;

namespace RagiFiler.Views.Templates
{
    class FilePreviewDataTemplateSelector : DataTemplateSelector
    {
        public FilePreviewDataTemplateSelector()
        {
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null || container == null)
            {
                return null;
            }

            if (!(container is FrameworkElement element))
            {
                return null;
            }

            if (!(item is FileListViewItemViewModel viewModel))
            {
                return GetDefaultFileNameTemplate(element);
            }

            if (viewModel.Item is FileInfo fileInfo)
            {
                switch (fileInfo.Extension.ToLower(CultureInfo.CurrentCulture))
                {
                    case ".avi":
                    case ".wmv":
                    case ".mkv":
                    case ".mp4":
                        return GetVideoFileNameTemplate(element);
                    default:
                        return GetDefaultFileNameTemplate(element);
                }
            }

            return GetDefaultFileNameTemplate(element);
        }

        private DataTemplate GetDefaultFileNameTemplate(FrameworkElement element)
        {
            return element.FindResource("DefaultFileNameTemplate") as DataTemplate;
        }

        private DataTemplate GetVideoFileNameTemplate(FrameworkElement element)
        {
            return element.FindResource("VideoFileNameTemplate") as DataTemplate;
        }
    }
}
