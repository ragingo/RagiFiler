using System.Collections.Generic;
using System.Windows;

namespace RagiFiler.Controls
{
    static class DependencyObjectExtensions
    {
        public static IEnumerable<T> GetChildren<T>(this DependencyObject obj, bool recursive = false)
            where T : DependencyObject
        {
            return VisualTreeHelperEx.GetChilden<T>(obj, recursive);
        }
    }

    static class FrameworkElementExtensions
    {
        public static T FindName<T>(this FrameworkElement obj, string name)
            where T : FrameworkElement
        {
            return obj.FindName(name) as T;
        }

        public static T FindChildByName<T>(this FrameworkElement obj, string name, bool recursive = false)
            where T : FrameworkElement
        {
            return VisualTreeHelperEx.FindChildByName<T>(obj, name, recursive);
        }
    }
}
