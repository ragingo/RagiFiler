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
                return GetDefaultFileTemplate(element);
            }

            if (viewModel.Item is FileInfo fileInfo)
            {
                switch (fileInfo.Extension.ToLower(CultureInfo.CurrentCulture))
                {
                    case ".avi":
                    case ".wmv":
                    case ".mkv":
                    case ".mp4":
                        return GetVideoFileTemplate(element);
                    default:
                        return GetDefaultFileTemplate(element);
                }
            }

            return GetDefaultFileTemplate(element);
        }

        private DataTemplate GetDefaultFileTemplate(FrameworkElement element)
        {
            return element.FindResource("DefaultFileTemplate") as DataTemplate;
        }

        private DataTemplate GetVideoFileTemplate(FrameworkElement element)
        {
            return element.FindResource("VideoFileTemplate") as DataTemplate;
        }
    }
}
