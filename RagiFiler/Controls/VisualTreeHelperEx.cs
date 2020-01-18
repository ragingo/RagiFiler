using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace RagiFiler.Controls
{
    static class VisualTreeHelperEx
    {
        public static IEnumerable<T> GetChilden<T>(DependencyObject obj, bool recursive = false)
            where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                if (!(VisualTreeHelper.GetChild(obj, i) is T child))
                {
                    continue;
                }

                yield return child;

                if (!recursive)
                {
                    continue;
                }

                foreach (var grandChild in GetChilden<T>(child))
                {
                    yield return grandChild;
                }
            }
        }

        public static T FindChildByName<T>(FrameworkElement element, string name, bool recursive = false)
            where T : FrameworkElement
        {
            return
                GetChilden<T>(element, recursive)
                    .Where(x => x.GetValue(FrameworkElement.NameProperty) as string == name)
                    .FirstOrDefault() as T;
        }

        public static void Dump(FrameworkElement obj)
        {
            Debug.WriteLine("[{0}] {1}", obj.GetType().Name, obj.Name);

            foreach (var item in GetChilden<DependencyObject>(obj))
            {
                if (item is FrameworkElement)
                {
                    var fe = item as FrameworkElement;
                    Debug.WriteLine("[{0}] {1}", fe.GetType().Name, fe.Name);
                }
                else
                {
                    Debug.WriteLine("[{0}] {1}", item.GetType().Name, "");
                }
            }
        }
    }
}
