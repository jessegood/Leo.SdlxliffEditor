using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using Leo.SdlxliffEditor.ContextMenus;
using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Leo.SdlxliffEditor.ViewModels;

public class QASegmentPairViewModel : ObservableObject, IQASegmentPairViewModel
{
    private readonly ISdlxliffDocument document;
    private readonly string fileName;
    private readonly int rowNumber;
    private readonly ISegmentPair segmentPair;

    public QASegmentPairViewModel(ISegmentPair segmentPair, ObservableCollection<ContextMenuItem> contextMenuItems,
        ISdlxliffDocument document, int rowNumber, string fileName = null)
    {
        this.segmentPair = segmentPair;
        ContextMenuItems = contextMenuItems;
        this.document = document;
        this.rowNumber = rowNumber;
        this.fileName = fileName;
    }

    public ConfirmationLevel ConfirmationLevel
    {
        get => segmentPair.ConfirmationLevel;
        set
        {
            if (SetProperty(segmentPair.ConfirmationLevel, value, segmentPair, (pair, c) => pair.ConfirmationLevel = c))
            {
                IsChanged = true;
            }
        }
    }

    public ObservableCollection<ContextMenuItem> ContextMenuItems { get; }

    public ISdlxliffDocument Document => document;

    public string FileName => fileName;

    public bool IsChanged { get; set; }

    public bool IsLocked
    {
        get => segmentPair.Locked;
        set
        {
            if (SetProperty(segmentPair.Locked, value, segmentPair, (pair, l) => pair.Locked = l))
            {
                IsChanged = true;
            }
        }
    }

    public byte MatchPercentage
    {
        get => segmentPair.Percent;
        set => SetProperty(segmentPair.Percent, value, segmentPair, (pair, p) => pair.Percent = p);
    }

    public string Origin
    {
        get => segmentPair.Origin;
        set => SetProperty(segmentPair.Origin, value, segmentPair, (pair, o) => pair.Origin = o);
    }

    public int RowNumber => rowNumber;

    public ISegment SourceSegment
    {
        get => segmentPair.SourceSegment;
        set => SetProperty(segmentPair.SourceSegment, value, segmentPair, (pair, s) => pair.SourceSegment = s);
    }

    public ISegment TargetSegment
    {
        get => segmentPair.TargetSegment;
        set => SetProperty(segmentPair.TargetSegment, value, segmentPair, (pair, s) => pair.TargetSegment = s);
    }

    public TextContextMatchLevel TextMatch
    {
        get => segmentPair.TextMatch;
        set => SetProperty(segmentPair.TextMatch, value, segmentPair, (pair, t) => pair.TextMatch = t);
    }

    public void AcceptChanges()
    {
        IsChanged = false;
    }
}