using Leo.SdlxliffEditor.Dialogs.ViewModels;
using Leo.SdlxliffEditor.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.Interfaces;

public interface IEditorViewModel
{
    event Action<string> FileChanged;

    event Action<int> FileSelected;

    public event Action<MatchInfo> FindTextExecuted;
    event Action<MatchInfo, string> ReplaceTextExecuted;

    CollectionViewSource SegmentPairsView { get; set; }

    SegmentPairViewModel SelectedItem { get; set; }
    bool HasUnsavedChanges { get; set; }

    void Close();

    void GoToFoundSegment(MatchInfo matchInfo);

    Task Save();

    void Show();

    void UpdateTitle(double offset);
    void ReplaceText(MatchInfo matchInfo, string replaceText);
    void ResetChangeTracker();
}