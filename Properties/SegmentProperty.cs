using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace Leo.SdlxliffEditor.Properties;

public static class SegmentProperty
{
    public static readonly DependencyProperty SegmentContentsProperty =
        DependencyProperty.RegisterAttached("SegmentContents", typeof(ISegment), typeof(SegmentProperty),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSegmentContentsChanged));

    public static ISegment GetSegmentContents(DependencyObject obj)
    {
        return (ISegment)obj.GetValue(SegmentContentsProperty);
    }

    public static void SetSegmentContents(DependencyObject obj, ISegment value)
    {
        obj.SetValue(SegmentContentsProperty, value);
    }

    private static void OnSegmentContentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RichTextBox rtb && e.NewValue is ISegment s)
        {
            RichTextBoxHelper.UpdateFlowDocument(rtb, s);
        }
    }
}
