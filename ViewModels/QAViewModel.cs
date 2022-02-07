using AdonisUI.Controls;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.QACheckers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Leo.SdlxliffEditor.ViewModels;

public class QAViewModel : ObservableObject
{
    private readonly IQACheckExecuterService checkExecuterService;
    private readonly ILoadFilesDialogViewModel loadFilesDialogViewModel;
    private readonly IMessageBoxService messageBoxService;
    private ObservableCollection<QACheckResultsViewModelBase> checkResults;
    private bool isProgressBarAnimationEnabled;
    private string progressBarContent;
    private double progressValue;

    public QAViewModel(IQACheckExecuterService checkExecuterService,
                       ILoadFilesDialogViewModel loadFilesDialogViewModel,
                       IMessageBoxService messageBoxService,
                       ICommandExceptionHandler commandExceptionHandler)
    {
        QASettingsViewModel = checkExecuterService.Settings;

        LoadFilesCommand = new RelayCommand(OnLoadFiles);
        RunQACheckCommand = new AsyncRelayCommand(OnRunQACheck, CanExecute);
        SaveFilesCommand = new AsyncRelayCommand(OnSaveFiles, CanExecute);

        commandExceptionHandler.RegisterCommands(SaveFilesCommand);

        this.checkExecuterService = checkExecuterService;
        this.loadFilesDialogViewModel = loadFilesDialogViewModel;
        this.messageBoxService = messageBoxService;
        LoadResults();
    }

    public bool HasUnsavedChanges { get; private set; }

    public bool IsProgressBarAnimationEnabled
    {
        get => isProgressBarAnimationEnabled;
        set => SetProperty(ref isProgressBarAnimationEnabled, value);
    }

    public IRelayCommand LoadFilesCommand { get; }

    public string ProgressBarContent
    {
        get => progressBarContent;
        set => SetProperty(ref progressBarContent, value);
    }

    public double ProgressValue
    {
        get => progressValue;
        set => SetProperty(ref progressValue, value);
    }

    public ObservableCollection<QACheckResultsViewModelBase> QACheckResults
    {
        get => checkResults;
        set => SetProperty(ref checkResults, value);
    }

    public IQASettingsViewModel QASettingsViewModel { get; }

    public IAsyncRelayCommand RunQACheckCommand { get; }

    public IAsyncRelayCommand SaveFilesCommand { get; }

    private static ObservableCollection<QACheckResultsViewModelBase> CreateCheckResultsViews(IEnumerable<QACheckerBase> qaCheckResults)
    {
        ObservableCollection<QACheckResultsViewModelBase> checkResultsViews = new();

        foreach (var qaCheckResult in qaCheckResults)
        {
            checkResultsViews.Add(qaCheckResult switch
            {
                TargetInconsistenciesQAChecker targetInconsistencies => new InconsistenciesQACheckResultsViewModel(targetInconsistencies),
                SourceInconsistenciesQAChecker sourceInconsistencies => new InconsistenciesQACheckResultsViewModel(sourceInconsistencies, false),
                _ => new QACheckResultsViewModel(qaCheckResult)
            });
        }

        return checkResultsViews;
    }

    private static void ResetChangeTracker(IDictionary<string, IList<IQASegmentPairViewModel>> segmentPairs)
    {
        foreach (var pair in segmentPairs)
        {
            foreach (var segmentPair in pair.Value)
            {
                segmentPair.AcceptChanges();
            }
        }
    }

    private bool CanExecute()
    {
        return checkExecuterService.HasFiles;
    }

    private async void CheckResultRemoved(QACheckResultsViewModelBase checkResult)
    {
        if (HasUnsavedChanges ||
            checkResult.SegmentPairs.Values.SelectMany(s => s).Any(s => s.IsChanged))
        {
            var result = messageBoxService.ShowUnsavedChangesMessageBox();
            if (result == MessageBoxResult.Yes)
            {
                await OnSaveFiles();
            }
        }

        checkResult.TargetChanged -= TargetChanged;
        checkResult.CheckResultRemoved -= CheckResultRemoved;

        checkExecuterService.RemoveResult(QACheckResults.IndexOf(checkResult));
        QACheckResults.Remove(checkResult);
    }

    private void DisplayResults(IEnumerable<QACheckerBase> qaCheckResults)
    {
        ObservableCollection<QACheckResultsViewModelBase> checkResultsViews = CreateCheckResultsViews(qaCheckResults);
        int fileCount = 0;

        foreach (var document in checkExecuterService.Documents)
        {
            string currentFile = checkExecuterService.SdlxliffFiles[fileCount++];
            int rowNumber = 0;

            foreach (var tu in document.TranslationUnits.Where(t => t.HasSegmentPairs))
            {
                foreach (var segmentPair in tu.GetSegmentPairs())
                {
                    rowNumber++;
                    foreach (var checkResultView in checkResultsViews)
                    {
                        checkResultView.Add(currentFile, rowNumber, segmentPair, document);
                    }
                }
            }
        }

        SetQACheckResults(checkResultsViews);
    }
    private void LoadResults()
    {
        var results = checkExecuterService.GetResults();
        if (results is not null)
        {
            DisplayResults(results);
        }
    }

    private void OnFileListCleared()
    {
        checkExecuterService.ClearFiles();
    }

    private void OnLoadFiles()
    {
        if (checkExecuterService.HasFiles)
        {
            loadFilesDialogViewModel.LoadStoredFiles(checkExecuterService.SdlxliffFiles);
        }

        loadFilesDialogViewModel.FileListCleared += OnFileListCleared;
        if (loadFilesDialogViewModel.ShowDialog() == true)
        {
            if (loadFilesDialogViewModel.HasFiles)
            {
                checkExecuterService.LoadFiles(loadFilesDialogViewModel.FilePaths);
            }
        }

        RunQACheckCommand.NotifyCanExecuteChanged();
        SaveFilesCommand.NotifyCanExecuteChanged();
    }

    private void OnProgressValueChanged(double progressValue)
    {
        ProgressValue = progressValue;
    }

    private async Task OnRunQACheck(CancellationToken cancellationToken)
    {
        ProgressBarContent = (string)App.Current.Resources["Processing"];
        IsProgressBarAnimationEnabled = true;
        var progress = new Progress<double>(OnProgressValueChanged);
        try
        {
            await checkExecuterService.ExecuteAsync(progress, cancellationToken);
            LoadResults();
        }
        catch (Exception e)
        {
            MessageBoxHelper.ShowErrorMessage(e);
        }
        finally
        {
            ProgressBarContent = (string)App.Current.Resources["ProcessingFinished"];
            IsProgressBarAnimationEnabled = false;
        }
    }

    private async Task OnSaveFiles()
    {
        await checkExecuterService.SaveFilesAsync();

        HasUnsavedChanges = false;

        foreach (var checkResultView in QACheckResults)
        {
            ResetChangeTracker(checkResultView.SegmentPairs);
        }
    }

    private void SetQACheckResults(ObservableCollection<QACheckResultsViewModelBase> checkResultsViews)
    {
        foreach (var checkResultView in checkResultsViews)
        {
            checkResultView.TargetChanged += TargetChanged;
            checkResultView.CheckResultRemoved += CheckResultRemoved;
            ResetChangeTracker(checkResultView.SegmentPairs);
        }

        QACheckResults = checkResultsViews;
    }

    private void TargetChanged()
    {
        HasUnsavedChanges = true;
    }
}