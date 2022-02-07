using Leo.Sdlxliff;
using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Model.Common;
using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;

namespace Leo.SdlxliffEditor.ViewModels;

public class DisplayFilterViewModel : ObservableObject, IDisplayFilterViewModel
{
    private readonly SegmentSearchSettings segmentSearchSettings;
    private List<string> fileList;
    private string segmentsFilteredStatus;
    private string source;
    private string target;

    public DisplayFilterViewModel()
    {
        FilterAttributes = FilterAttributesViewModel.CreateFilterAttributes();

        ApplyFilterCommand = new RelayCommand(ApplyFilter);
        ClearFilterCommand = new RelayCommand(ClearFilter);
        FileSelectedCommand = new RelayCommand<string>(GoToFile);

        segmentSearchSettings = new SegmentSearchSettings
        {
            SegmentStatus = new SegmentStatus()
        };
    }

    public event Action<string> FileSelected;

    public event Action FilterApplied;

    public event Action FilterCleared;

    public ICommand ApplyFilterCommand { get; }

    public ICommand ClearFilterCommand { get; }

    public List<string> FileList
    {
        get => fileList;
        set => SetProperty(ref fileList, value);
    }

    public RelayCommand<string> FileSelectedCommand { get; }

    public List<FilterAttributesViewModel> FilterAttributes { get; }

    public bool IsCaseSensitive
    {
        get => segmentSearchSettings.CaseSensitive;
        set => SetProperty(segmentSearchSettings.CaseSensitive, value, segmentSearchSettings, (s, c) => s.CaseSensitive = c);
    }

    public bool SearchInTags
    {
        get => segmentSearchSettings.SearchInTags;
        set => SetProperty(segmentSearchSettings.SearchInTags, value, segmentSearchSettings, (s, t) => s.SearchInTags = t);
    }

    public string SegmentsFilteredStatus
    {
        get => segmentsFilteredStatus;
        set => SetProperty(ref segmentsFilteredStatus, value);
    }

    public string Source
    {
        get => source;
        set => SetProperty(ref source, value);
    }

    public string Target
    {
        get => target;
        set => SetProperty(ref target, value);
    }

    public bool UseRegularExpression
    {
        get => segmentSearchSettings.UseRegularExpressions;
        set => SetProperty(segmentSearchSettings.UseRegularExpressions, value, segmentSearchSettings, (s, r) => s.UseRegularExpressions = r);
    }

    public void Filter(object sender, FilterEventArgs e)
    {
        if (e.Item is ISegmentPairViewModel segmentPair)
        {
            if (SegmentSearchHelper.Search(Source, Target, segmentPair.SegmentPair, segmentSearchSettings))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }
    }

    public void SetFileList(IEnumerable<string> fileList)
    {
        FileList = new List<string>(fileList);
    }

    public void SetSegmentCount(int count, int total)
    {
        SegmentsFilteredStatus = $"Filtered {count} of {total} segments";
    }

    private void ApplyFilter()
    {
        UpdateSegmentSearchSettings();

        FilterApplied?.Invoke();
    }

    private void ClearFilter()
    {
        FilterCleared?.Invoke();
    }

    private SegmentStatus.LockSearchSettings GetLockSearchSettings()
    {
        SegmentStatus.LockSearchSettings lockSearchSettings = SegmentStatus.LockSearchSettings.Unspecified;
        var locking = FilterAttributes[4].Children;

        if (locking[0].IsChecked == true)
        {
            lockSearchSettings = SegmentStatus.LockSearchSettings.Locked;
        }

        if (locking[0].IsChecked == true)
        {
            lockSearchSettings = SegmentStatus.LockSearchSettings.Locked;
        }

        return lockSearchSettings;
    }

    private SegmentStatus.MatchLevel GetMatchLevelFilterSettings()
    {
        SegmentStatus.MatchLevel matchLevel = SegmentStatus.MatchLevel.None;
        var origin = FilterAttributes[1].Children;

        if (origin[0].IsChecked == true)
        {
            matchLevel |= SegmentStatus.MatchLevel.PerfectMatch;
        }

        if (origin[1].IsChecked == true)
        {
            matchLevel |= SegmentStatus.MatchLevel.ContextMatch;
        }

        if (origin[2].IsChecked == true)
        {
            matchLevel |= SegmentStatus.MatchLevel.AutomatedTranslation;
        }

        if (origin[3].IsChecked == true)
        {
            segmentSearchSettings.SegmentStatus.MatchPercentage = (p) => p == 100;
        }

        if (origin[4].IsChecked == true)
        {
            if (origin[3].IsChecked == true)
            {
                segmentSearchSettings.SegmentStatus.MatchPercentage = (p) => p == 100 || p < 100;
            }
            else
            {
                segmentSearchSettings.SegmentStatus.MatchPercentage = (p) => p < 100;
            }
        }

        if (origin[5].IsChecked == true)
        {
            matchLevel |= SegmentStatus.MatchLevel.Interactive;
        }

        if (origin[6].IsChecked == true)
        {
            matchLevel |= SegmentStatus.MatchLevel.CopiedFromSource;
        }

        if (origin[7].IsChecked == true)
        {
            matchLevel |= SegmentStatus.MatchLevel.AutoPropagated;
        }

        if (origin[8].IsChecked == true)
        {
            matchLevel |= SegmentStatus.MatchLevel.NeuralMachineTranslation;
        }

        return matchLevel;
    }

    private RepetitionInfo GetRepetitionFilterSettings()
    {
        var repetitions = FilterAttributes[2].Children;
        var repetitionInfo = RepetitionInfo.None;
        if (repetitions[0].IsChecked == true)
        {
            repetitionInfo |= RepetitionInfo.FirstOccurence | RepetitionInfo.NotFirstOccurence;
        }

        if (repetitions[1].IsChecked == true)
        {
            repetitionInfo |= RepetitionInfo.FirstOccurence;
        }

        if (repetitions[2].IsChecked == true)
        {
            repetitionInfo |= RepetitionInfo.NotFirstOccurence;
        }

        if (repetitions[3].IsChecked == true)
        {
            repetitionInfo |= RepetitionInfo.FirstOccurence | RepetitionInfo.None;
        }

        return repetitionInfo;
    }

    private ConfirmationLevel GetSegmentStatusFilterSettings()
    {
        ConfirmationLevel confirmationLevel = ConfirmationLevel.Unspecified;
        var status = FilterAttributes[0].Children;
        if (status[0].IsChecked == true)
        {
            confirmationLevel |= ConfirmationLevel.NotTranslated;
        }

        if (status[1].IsChecked == true)
        {
            confirmationLevel |= ConfirmationLevel.Draft;
        }

        if (status[2].IsChecked == true)
        {
            confirmationLevel |= ConfirmationLevel.Translated;
        }

        if (status[3].IsChecked == true)
        {
            confirmationLevel |= ConfirmationLevel.RejectedTranslation;
        }

        if (status[4].IsChecked == true)
        {
            confirmationLevel |= ConfirmationLevel.ApprovedTranslation;
        }

        if (status[5].IsChecked == true)
        {
            confirmationLevel |= ConfirmationLevel.RejectedSignOff;
        }

        if (status[6].IsChecked == true)
        {
            confirmationLevel |= ConfirmationLevel.ApprovedSignOff;
        }

        return confirmationLevel;
    }

    private void GoToFile(string fileName)
    {
        FileSelected?.Invoke(fileName);
    }
    private void UpdateSegmentSearchSettings()
    {
        // Confirmation level
        segmentSearchSettings.SegmentStatus.ConfirmationLevel = GetSegmentStatusFilterSettings();
        // Match status
        segmentSearchSettings.SegmentStatus.MatchStatus = GetMatchLevelFilterSettings();
        // Repetitions
        segmentSearchSettings.SegmentStatus.RepetitionStatus = GetRepetitionFilterSettings();
        // Comments and revisions
        var review = FilterAttributes[3].Children;
        segmentSearchSettings.WithComments = review[0].IsChecked == true;
        segmentSearchSettings.WithRevisions = review[1].IsChecked == true;
        // Lock status
        segmentSearchSettings.SegmentStatus.LockedStatus = GetLockSearchSettings();
    }
}