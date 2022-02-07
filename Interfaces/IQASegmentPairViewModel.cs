using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using Leo.SdlxliffEditor.ContextMenus;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Leo.SdlxliffEditor.Interfaces;

public interface IQASegmentPairViewModel : IChangeTracking
{
    ConfirmationLevel ConfirmationLevel { get; set; }
    ObservableCollection<ContextMenuItem> ContextMenuItems { get; }
    ISdlxliffDocument Document { get; }
    string FileName { get; }
    bool IsLocked { get; set; }
    byte MatchPercentage { get; set; }
    string Origin { get; set; }
    int RowNumber { get; }
    ISegment SourceSegment { get; set; }
    ISegment TargetSegment { get; set; }
    TextContextMatchLevel TextMatch { get; set; }
}