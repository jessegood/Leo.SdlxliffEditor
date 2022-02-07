using Leo.SdlxliffEditor.BatchTasks;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Leo.SdlxliffEditor.ViewModels;

public class BatchTaskViewModel : ObservableObject
{
    private readonly IBatchTaskExecuterService batchTaskExecuterService;
    private readonly ISettingsDialogService settingsDialogService;
    private ObservableCollection<string> filePaths;
    private bool isProgressBarAnimationEnabled;
    private string progressBarContent;
    private double progressValue;

    public BatchTaskViewModel(IBatchTaskExecuterService batchTaskExecuterService, ISettingsDialogService settingsDialogService)
    {
        this.batchTaskExecuterService = batchTaskExecuterService;
        this.settingsDialogService = settingsDialogService;

        DropCommand = new RelayCommand<object>(OnDrop);
        SelectFolderCommand = new RelayCommand(OnSelectFolder);
        SelectFilesCommand = new RelayCommand(OnSelectFiles);
        ClearListCommand = new RelayCommand(OnClearList, CanClearListExecute);
        RunBatchTaskCommand = new AsyncRelayCommand(OnRunBatchTask, CanRunBatchTaskExecute);

        BatchTasks = LoadBatchTasks();
        batchTaskExecuterService.AddBatchTasks(BatchTasks);
    }

    public ObservableCollection<BatchTaskBase> BatchTasks { get; }

    public IRelayCommand ClearListCommand { get; }

    public IRelayCommand<object> DropCommand { get; }

    public ObservableCollection<string> FilePaths
    {
        get => filePaths;
        set => SetProperty(ref filePaths, value);
    }

    public bool IsProgressBarAnimationEnabled
    {
        get => isProgressBarAnimationEnabled;
        set => SetProperty(ref isProgressBarAnimationEnabled, value);
    }

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

    public IAsyncRelayCommand RunBatchTaskCommand { get; }

    public IRelayCommand SelectFilesCommand { get; }

    public IRelayCommand SelectFolderCommand { get; }

    private bool HasFiles => FilePaths is { Count: > 0 };

    private bool CanClearListExecute()
    {
        return HasFiles;
    }

    private bool CanRunBatchTaskExecute()
    {
        return HasFiles && batchTaskExecuterService.IsTaskChecked;
    }

    private ObservableCollection<BatchTaskBase> LoadBatchTasks()
    {
        ObservableCollection<BatchTaskBase> batchTasks = new();

        batchTasks.Add(new HideSegmentsBatchTask(settingsDialogService, NotifyCommands));
        batchTasks.Add(new UnhideSegmentsBatchTask(settingsDialogService, NotifyCommands));

        return batchTasks;
    }

    private void LoadFilesAndNotifyCommands()
    {
        if (HasFiles)
        {
            batchTaskExecuterService.LoadFiles(FilePaths);
        }

        NotifyCommands();
    }

    private void NotifyCommands()
    {
        RunBatchTaskCommand.NotifyCanExecuteChanged();
        ClearListCommand.NotifyCanExecuteChanged();
    }

    private void OnClearList()
    {
        FilePaths.Clear();
        NotifyCommands();
    }

    private void OnDrop(object data)
    {
        FilePaths = DragAndDropHelper.GetSdlxliffs(data);
        LoadFilesAndNotifyCommands();
    }

    private void OnProgressValueChanged(double progressValue)
    {
        ProgressValue = progressValue;
    }

    private async Task OnRunBatchTask(CancellationToken cancellationToken)
    {
        RunBatchTaskCommand.NotifyCanExecuteChanged();
        ProgressBarContent = (string)App.Current.Resources["Processing"];
        IsProgressBarAnimationEnabled = true;
        var progress = new Progress<double>(OnProgressValueChanged);

        try
        {
            await batchTaskExecuterService.ExecuteAsync(progress, cancellationToken);
            await batchTaskExecuterService.SaveFilesAsync();
        }
        catch (Exception e)
        {
            MessageBoxHelper.ShowErrorMessage(e);
        }
        finally
        {
            RunBatchTaskCommand.NotifyCanExecuteChanged();
            ProgressBarContent = (string)App.Current.Resources["ProcessingFinished"];
            IsProgressBarAnimationEnabled = false;
        }
    }

    private void OnSelectFiles()
    {
        FilePaths = DialogHelper.ShowOpenFileDialog();
        LoadFilesAndNotifyCommands();
    }

    private void OnSelectFolder()
    {
        FilePaths = DialogHelper.ShowFolderBrowserDialog();
        LoadFilesAndNotifyCommands();
    }
}