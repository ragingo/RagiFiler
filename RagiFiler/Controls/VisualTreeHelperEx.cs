using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Controls;

namespace RagiFiler.Controls
{
    static class VisualTreeHelperEx
    {
        public static IEnumerable<DependencyObject> GetChilden(DependencyObject obj, bool recursive = false)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                yield return child;

                if (!recursive)
                {
                    continue;
                }

                foreach (var grandChild in GetChilden(child))
                {
                    yield return grandChild;
                }
            }
        }

        public static T FindChild<T>(DependencyObject obj, string name, bool recursive = false)
            where T : DependencyObject
        {
            return
                GetChilden(obj, recursive)
                    .Where(x => x.GetValue(FrameworkElement.NameProperty) as string == name)
                    .FirstOrDefault() as T;
        }

        public static void Dump(FrameworkElement obj)
        {
            Debug.WriteLine("[{0}] {1}", obj.GetType().Name, obj.Name);

            foreach (var item in GetChilden(obj))
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
