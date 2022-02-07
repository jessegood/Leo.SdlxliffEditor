using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using Leo.SdlxliffEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Leo.SdlxliffEditor.Interfaces;

public interface ISegmentPairViewModel : IChangeTracking
{
    ObservableCollection<CommentGroupViewModel> Comments { get; }
    
    ConfirmationLevel ConfirmationLevel { get; set; }
    
    ISdlxliffDocument Document { get; }
    
    bool HasComments { get; set; }
    
    bool IsLocked { get; set; }

    public bool IsSourceChanged { get; set; }

    public bool IsTargetChanged { get; set; }
    
    byte MatchPercentage { get; set; }

    string Origin { get; set; }

    IEnumerable<KeyValuePair<string, string>> PreviousOrigin { get; }

    int RowNumber { get; }

    ISegmentPair SegmentPair { get; }
    
    ISegment SourceSegment { get; set; }

    ISegment TargetSegment { get; set; }

    TextContextMatchLevel TextMatch { get; set; }

    event Action SegmentChanged;

    void UpdateCanExecute();
}
