using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.Dialogs.ViewModels;
using Leo.SdlxliffEditor.Interfaces;
using System;

namespace Leo.SdlxliffEditor.BatchTasks;

public class HideSegmentsBatchTask : BatchTaskBase
{
    private HideSegmentsSettingsViewModel hideSegmentsViewModel;

    public HideSegmentsBatchTask(ISettingsDialogService settingsDialogService, Action notifyCommands)
        : base(settingsDialogService, notifyCommands)
    {
    }

    public override string Description { get; set; } = (string)App.Current.Resources["SegmentHideBatchTaskDescription"];

    public override string Name { get; set; } = (string)App.Current.Resources["SegmentHideBatchTask"];

    public override void ProcessSegmentPair(ISegmentPair segmentPair)
    {
        if (hideSegmentsViewModel is not null)
        {
            if (IsMatch(segmentPair))
            {
                // Since a translation unit can contain multiple pairs
                // we need to avoid duplicating the prefix
                if (!segmentPair.Parent.TranslationUnitId.StartsWith("lockTU_Leo"))
                {
                    segmentPair.Parent.TranslationUnitId = "lockTU_Leo" + segmentPair.Parent.TranslationUnitId;
                }
            }
        }
    }

    protected override void OnSettingsActivated()
    {
        var segmentsSettings = new HideSegmentsSettingsViewModel();
        var dialog = SettingsDialogService.CreateSettingsDialog(segmentsSettings);
        // Store settings if user hits OK
        if (dialog.ShowDialog() == true)
        {
            hideSegmentsViewModel = segmentsSettings;
        }
    }

    private bool IsMatch(ISegmentPair segmentPair)
    {
        if (segmentPair.Parent.GetSegmentPairs().Count == 1)
        {
            return IsMatchVerifier(segmentPair);
        }
        // A translation unit can contain multiple segment pairs
        // we make sure the conditions are true for all pairs to hide them
        else
        {
            bool isMatch = true;

            foreach (var segPair in segmentPair.Parent.GetSegmentPairs())
            {
                isMatch &= IsMatchVerifier(segPair);
            }

            return isMatch;
        }
    }

    private bool IsMatchVerifier(ISegmentPair segmentPair)
    {
        if (hideSegmentsViewModel.LockedSegments)
        {
            return segmentPair.Locked;
        }

        if (hideSegmentsViewModel.PerfectMatchSegments)
        {
            return SegmentSearchHelper.IsPerfectMatch(segmentPair);
        }

        if (hideSegmentsViewModel.ContextMatchSegments)
        {
            return SegmentSearchHelper.IsContextMatch(segmentPair);
        }

        if (hideSegmentsViewModel.OneHundredMatchSegments)
        {
            return segmentPair.Percent >= 100;
        }

        return false;
    }
}
