using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ChessWPF.HelperClasses.ControlGetters
{
    public static class ControlChildElementsFetcher
    {
        public static List<T> GetChildrenOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
            {
                return null!;
            }
            var foundChildren = new List<T>();

            var childrenCount = VisualTreeHelper.GetChildrenCount(depObj);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child.GetType() == typeof(T))
                {
                    foundChildren.Add((T)child);
                }
                else
                {
                    break;
                }
            }
            return foundChildren;
        }
    }
}
