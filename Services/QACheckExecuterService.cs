using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.QACheckers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Leo.SdlxliffEditor.Services;

public class QACheckExecuterService : ExecuterServiceBase, IQACheckExecuterService
{
    private List<QACheckerBase> checkItems;

    public QACheckExecuterService(IQASettingsViewModel settings)
    {
        Settings = settings;
    }

    public IQASettingsViewModel Settings { get; }

    public override async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        checkItems = CreateCheckItems();

        var currentFileIndex = 0;
        var currentProgress = 0;
        var progressInterval = 100 / SdlxliffFiles.Count;

        await foreach (var segmentPairs in LoadSegmentPairsAsync())
        {
            var currentSegmentIndex = 0;
            foreach (var segmentPair in segmentPairs)
            {
                currentSegmentIndex++;
                if (!ShouldExclude(segmentPair))
                {
                    Check(segmentPair, SdlxliffFiles[currentFileIndex], currentSegmentIndex);
                }
            }

            currentFileIndex++;
            currentProgress += progressInterval;
            progress.Report(currentProgress);
        }

        progress.Report(100);
    }

    public IEnumerable<QACheckerBase> GetResults()
    {
        return checkItems;
    }

    public void RemoveResult(int index)
    {
        checkItems.RemoveAt(index);
    }

    private void Check(ISegmentPair segmentPair, string fileName, int currentSegmentIndex)
    {
        foreach (var checkItem in checkItems)
        {
            checkItem.Check(segmentPair, fileName, currentSegmentIndex);
        }
    }

    private List<QACheckerBase> CreateCheckItems()
    {
        List<QACheckerBase> checkItems = new();

        foreach (var topCheckItem in Settings.QACheckItems)
        {
            if (topCheckItem.IsChecked is null or true)
            {
                foreach (var checkItem in topCheckItem.Children)
                {
                    if (checkItem.IsChecked is null or true)
                    {
                        checkItems.Add(QACheckerFactory.CreateQAChecker(Settings.CaseSensitive, checkItem));
                    }
                }
            }
        }

        return checkItems;
    }

    private bool ShouldExclude(ISegmentPair segmentPair)
    {
        if (Settings.ExcludeEmptySegments &&
            segmentPair.TargetSegment.Contents.Count == 0)
        {
            return true;
        }

        if (Settings.ExcludeLockedSegments &&
            segmentPair.Locked)
        {
            return true;
        }

        if (Settings.ExcludeContextMatchSegments &&
            SegmentSearchHelper.IsContextMatch(segmentPair))
        {
            return true;
        }

        if (Settings.ExcludePerfectMatchSegments &&
            SegmentSearchHelper.IsPerfectMatch(segmentPair))
        {
            return true;
        }

        if (Settings.ExcludeOneHundredMatchSegments &&
            segmentPair.Percent >= 100)
        {
            return true;
        }

        return false;
    }
}