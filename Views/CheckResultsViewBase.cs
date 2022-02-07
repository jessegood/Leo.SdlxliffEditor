using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.Exceptions;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Leo.SdlxliffEditor.Views;

public abstract class CheckResultsViewBase : UserControl
{
    public static readonly DependencyProperty IsTargetChangedProperty =
        DependencyProperty.RegisterAttached("IsTargetChanged", typeof(bool), typeof(CheckResultsViewBase),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool IsTargetChanged
    {
        get { return (bool)GetValue(IsTargetChangedProperty); }
        set { SetValue(IsTargetChangedProperty, value); }
    }

    public static bool GetIsTargetChanged(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsTargetChangedProperty);
    }

    public static void SetIsTargetChanged(DependencyObject obj, bool value)
    {
        obj.SetValue(IsTargetChangedProperty, value);
    }

    protected void OnTargetGotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            rtb.TextChanged += OnTargetTextChanged;
        }
    }

    protected void OnTargetLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            rtb.TextChanged -= OnTargetTextChanged;

            if (rtb.Tag is IQASegmentPairViewModel segmentPair)
            {
                if (IsTargetChanged)
                {
                    UpdateSegment(rtb.Document, segmentPair.TargetSegment, segmentPair.Document, segmentPair.RowNumber);
                }

                rtb.Tag = null;
            }
        }
    }

    protected void OnTargetTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            // We set the tag to the data context
            // in case the data context gets set to disconnected item
            IsTargetChanged = true;
            rtb.Tag = rtb.DataContext;
        }
    }

    private void UpdateSegment(FlowDocument document, ISegment segment, ISdlxliffDocument sdlxliffDocument, int rowNumber)
    {
        try
        {
            SegmentConverter.Update(document, segment, sdlxliffDocument);
        }
        catch (Exception ex) when (ex is MissingTagException or AggregateException)
        {
            ex.Data.Add("RowNumber", rowNumber);
            MessageBoxHelper.ShowErrorMessage(ex);
        }
        finally
        {
            IsTargetChanged = false;
        }
    }
}