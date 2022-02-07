using DiffMatchPatch;
using Leo.Sdlxliff.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Leo.SdlxliffEditor.Controls;

public partial class SegmentDiffViewer : UserControl
{
    public static readonly DependencyProperty IsTargetInconsistenciesProperty =
        DependencyProperty.Register("IsTargetInconsistencies", typeof(bool), typeof(SegmentDiffViewer), new FrameworkPropertyMetadata(false));

    public static readonly DependencyProperty SegmentPairsProperty =
        DependencyProperty.Register("SegmentPairs", typeof(List<IQASegmentPairViewModel>), typeof(SegmentDiffViewer),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnSegmentPairsPropertyChanged));

    public SegmentDiffViewer()
    {
        InitializeComponent();
    }

    public bool IsTargetInconsistencies
    {
        get { return (bool)GetValue(IsTargetInconsistenciesProperty); }
        set { SetValue(IsTargetInconsistenciesProperty, value); }
    }

    public List<IQASegmentPairViewModel> SegmentPairs
    {
        get { return (List<IQASegmentPairViewModel>)GetValue(SegmentPairsProperty); }
        set { SetValue(SegmentPairsProperty, value); }
    }

    private static TextBlock CreateTextBlock(List<Diff> diffs, bool useInsert = true)
    {
        TextBlock textBlock = new()
        {
            Margin = new Thickness(8)
        };

        foreach (var diff in diffs)
        {
            if (diff.Operation.Tag == Operation.Tags.Equal)
            {
                textBlock.Inlines.Add(new Run(diff.Text));
            }
            else if (useInsert && diff.Operation.Tag == Operation.Tags.Insert)
            {
                textBlock.Inlines.Add(new Run(diff.Text)
                {
                    Background = Brushes.Yellow
                });
            }
            else if (!useInsert && diff.Operation.Tag == Operation.Tags.Delete)
            {
                textBlock.Inlines.Add(new Run(diff.Text)
                {
                    Background = Brushes.Yellow
                });
            }
        }

        return textBlock;
    }

    private static void OnSegmentPairsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SegmentDiffViewer segmentDiffViewer && e.NewValue is List<IQASegmentPairViewModel> segmentPairs)
        {
            // Clear all children
            segmentDiffViewer.DiffPanel.Children.Clear();

            foreach (var stackPanel in segmentDiffViewer.CreateTextBlocks(segmentPairs))
            {
                segmentDiffViewer.DiffPanel.Children.Add(stackPanel);
            }
        }
    }

    private IEnumerable<StackPanel> CreateTextBlocks(List<IQASegmentPairViewModel> segmentPairs)
    {
        // Group by source or target content and then order by the count
        var inconsistencyGroups = segmentPairs.GroupBy(
            segmentPair => IsTargetInconsistencies ? segmentPair.TargetSegment.ToStringWithoutCommentsAndRevisions() :
                            segmentPair.SourceSegment.ToStringWithoutCommentsAndRevisions(),
            (key, pairs) => new { Key = key, Count = pairs.Count() }
            ).OrderByDescending(pair => pair.Count);

        var oldValue = inconsistencyGroups.First().Key;

        List<Diff> deletions = new();
        List<StackPanel> textBlocks = new();

        foreach (var newValue in inconsistencyGroups.Skip(1))
        {
            var differ = DiffMatchPatchModule.Default;
            var diffs = differ.DiffMain(oldValue, newValue.Key);

            deletions.AddRange(diffs.Where(d => d.Operation.Tag == Operation.Tags.Delete));

            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(CreateTextBlock(diffs, false));
            stackPanel.Children.Add(CreateTextBlock(diffs));
            textBlocks.Add(stackPanel);
        }

        return textBlocks;
    }
}