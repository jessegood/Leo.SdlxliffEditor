using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using Leo.SdlxliffEditor.Commands;
using Leo.SdlxliffEditor.ContextMenus;
using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Leo.SdlxliffEditor.ViewModels;

public class SegmentPairViewModel : ObservableObject, ISegmentPairViewModel
{
    private readonly ISdlxliffDocument document;
    private readonly int rowNumber;
    private readonly ISegmentPair segmentPair;
    private ObservableCollection<CommentGroupViewModel> comments;
    private bool hasComments;
    private bool isSourceChanged;
    private bool isTargetChanged;

    public SegmentPairViewModel(ISegmentPair segmentPair,
                                        ObservableCollection<ContextMenuItem> contextMenuItems, ISdlxliffDocument document)
    {
        this.segmentPair = segmentPair;
        this.rowNumber = int.Parse(segmentPair.Id);
        this.document = document;
        ContextMenuItems = CreateSegmentPairContextMenuItems(contextMenuItems);
        InitializeComments();
    }

    public event Action SegmentChanged;

    public ObservableCollection<CommentGroupViewModel> Comments
    {
        get => comments;
        set => SetProperty(ref comments, value);
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

    public bool HasComments
    {
        get => hasComments;
        set => SetProperty(ref hasComments, value);
    }

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

    public bool IsOriginVisible => PreviousOrigin != null && segmentPair.Previous.Values.Count > 0;

    public bool IsSourceChanged
    {
        get => isSourceChanged;
        set
        {
            isSourceChanged = value;
            if (value is true)
            {
                SegmentChanged?.Invoke();
            }
        }
    }

    public bool IsTargetChanged
    {
        get => isTargetChanged;
        set
        {
            isTargetChanged = value;
            if (value is true)
            {
                SegmentChanged?.Invoke();
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

    // Skip over first item which is a hash code
    public IEnumerable<KeyValuePair<string, string>> PreviousOrigin => segmentPair.Previous?.Values.Skip(1);

    public int RowNumber => rowNumber;

    public ISegmentPair SegmentPair => segmentPair;

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

    public void UpdateCanExecute()
    {
        // The add comment command CanExecute needs to be called on every right click
        ContextMenuItems.Last().Command.NotifyCanExecuteChanged();
    }

    private ObservableCollection<ContextMenuItem> CreateSegmentPairContextMenuItems(ObservableCollection<ContextMenuItem> contextMenuItems)
    {
        var segmentPairContextMenuItems = new ObservableCollection<ContextMenuItem>(contextMenuItems)
            {
                new ContextMenuItem(App.Current.Resources["AddComment"] as string,
                                                     new AddCommentCommand(this))
            };

        return segmentPairContextMenuItems;
    }

    private void InitializeComments()
    {
        comments = new ObservableCollection<CommentGroupViewModel>();
        int index = 1;

        // Source comments
        foreach (var comment in segmentPair.SourceSegment.AllContent().Where(c => c.ContentType == TranslationUnitContentType.CommentsMarker))
        {
            comments.Add(new CommentGroupViewModel(index++, (CommentMarker)comment, this, true));
        }

        // Target comments
        foreach (var comment in segmentPair.TargetSegment.AllContent().Where(c => c.ContentType == TranslationUnitContentType.CommentsMarker))
        {
            comments.Add(new CommentGroupViewModel(index++, (CommentMarker)comment, this));
        }

        // Set boolean
        HasComments = Comments.Count > 0;
    }
}