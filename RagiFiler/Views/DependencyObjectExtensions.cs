using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace RagiFiler.Views
{
    static class DependencyObjectExtensions
    {
        public static IEnumerable<DependencyObject> GetChildren(this DependencyObject dependencyObject)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(dependencyObject);
            for (int i = 0; i < childrenCount; i++)
            {
                yield return VisualTreeHelper.GetChild(dependencyObject, i);
            }
        }
    }
}
