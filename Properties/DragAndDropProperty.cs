using Leo.SdlxliffEditor.Helpers;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Leo.SdlxliffEditor.Properties;

public static class DragAndDropProperty
{
    public static readonly DependencyProperty DragAndDropCommandProperty =
        DependencyProperty.RegisterAttached("DragAndDropCommand", typeof(ICommand), typeof(DragAndDropProperty), new PropertyMetadata(null, DragAndDropCommandPropertyChanged));

    public static ICommand GetDragAndDropCommand(DependencyObject obj)
    {
        return (ICommand)obj.GetValue(DragAndDropCommandProperty);
    }

    public static void SetDragAndDropCommand(DependencyObject obj, ICommand value)
    {
        obj.SetValue(DragAndDropCommandProperty, value);
    }

    private static void DragAndDropCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement element)
        {
            element.DragEnter += OnDragEnter;
            element.DragOver += OnDragOver;
            element.Drop += OnDrop;
        }
    }

    private static void HandleDragEvent(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var paths = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (IsValidDrag(paths))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
                return;
            }
        }

        e.Effects = DragDropEffects.None;
        e.Handled = true;
    }

    private static bool IsValidDrag(string[] paths)
    {
        return paths.All(path =>
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            else if (File.Exists(path))
            {
                return DragAndDropHelper.IsSdlxliff(path);
            }

            return false;
        });
    }

    private static void OnDragEnter(object sender, DragEventArgs e)
    {
        HandleDragEvent(sender, e);
    }

    private static void OnDragOver(object sender, DragEventArgs e)
    {
        HandleDragEvent(sender, e);
    }

    private static void OnDrop(object sender, DragEventArgs e)
    {
        if (sender is DependencyObject d)
        {
            if (e.AllowedEffects.HasFlag(DragDropEffects.Copy))
            {
                var target = d.GetValue(DragAndDropCommandProperty);
                if (target is ICommand command)
                {
                    command.Execute(e.Data);
                    e.Handled = true;
                }
            }
        }
    }
}
