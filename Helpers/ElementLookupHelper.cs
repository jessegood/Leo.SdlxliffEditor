using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Leo.SdlxliffEditor.Helpers;

public static class ElementLookupHelper
{
    public static T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        var currentParent = VisualTreeHelper.GetParent(child);

        while (currentParent != null)
        {
            if (currentParent is T foundParent)
            {
                return foundParent;
            }

            currentParent = VisualTreeHelper.GetParent(currentParent);
        }

        return null;
    }

    // Credits: https://stackoverflow.com/a/10279201/906773
    public static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);

            var result = (child as T) ?? GetChildOfType<T>(child);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public static IEnumerable<T> GetChildrenOfType<T>(DependencyObject depObj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);

            if (child is T t)
            {
                yield return t;
            }

            foreach (var descendent in GetChildrenOfType<T>(child))
            {
                yield return descendent;
            }
        }
    }

    public static T GetLastChildOfType<T>(DependencyObject depObj) where T : DependencyObject
    {
        T result = null;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);

            result = (child as T) ?? result;
            result = GetLastChildOfType<T>(child) ?? result;
        }

        return result;
    }
}
