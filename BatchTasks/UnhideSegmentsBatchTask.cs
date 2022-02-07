using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.Interfaces;
using System;

namespace Leo.SdlxliffEditor.BatchTasks;

public class UnhideSegmentsBatchTask : BatchTaskBase
{
    public UnhideSegmentsBatchTask(ISettingsDialogService settingsDialogService, Action notifyCommands)
        : base(settingsDialogService, notifyCommands)
    {
    }

    public override string Description { get; set; } = (string)App.Current.Resources["SegmentUnHideBatchTaskDescription"];
    public override string Name { get; set; } = (string)App.Current.Resources["SegmentUnHideBatchTask"];

    public override void ProcessSegmentPair(ISegmentPair segmentPair)
    {
        if (segmentPair.Parent.TranslationUnitId.StartsWith("lockTU_Leo"))
        {
            segmentPair.Parent.TranslationUnitId = segmentPair.Parent.TranslationUnitId.Remove(0, "lockTU_Leo".Length);
        }
    }

    protected override void OnSettingsActivated()
    {
        throw new NotImplementedException();
    }

    protected override bool SettingsCanExecute()
    {
        return false;
    }
}
