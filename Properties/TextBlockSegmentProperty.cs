using Leo.Sdlxliff.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace Leo.SdlxliffEditor.Properties;

public static class TextBlockSegmentProperty
{
    public static readonly DependencyProperty TextBlockSegmentContentsProperty =
        DependencyProperty.RegisterAttached("TextBlockSegmentContents", typeof(ISegment), typeof(TextBlockSegmentProperty),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnTextBlockSegmentContentsChanged));

    public static ISegment GetTextBlockSegmentContents(DependencyObject obj)
    {
        return (ISegment)obj.GetValue(TextBlockSegmentContentsProperty);
    }

    public static void SetTextBlockSegmentContents(DependencyObject obj, ISegment value)
    {
        obj.SetValue(TextBlockSegmentContentsProperty, value);
    }

    private static void OnTextBlockSegmentContentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBlock tb && e.NewValue is ISegment s)
        {
            tb.Text = s.ToString();
        }
    }
}