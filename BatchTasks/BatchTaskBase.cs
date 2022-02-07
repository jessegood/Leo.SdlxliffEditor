using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;

namespace Leo.SdlxliffEditor.BatchTasks;

public abstract class BatchTaskBase : ObservableObject, IBatchTask
{
    private readonly Action notifyCommands;
    private bool isChecked;
    public BatchTaskBase(ISettingsDialogService settingsDialogService, Action notifyCommands)
    {
        SettingsDialogService = settingsDialogService;
        this.notifyCommands = notifyCommands;

        SettingsCommand = new RelayCommand(OnSettingsActivated, SettingsCanExecute);
    }

    public abstract string Description { get; set; }

    public bool IsChecked
    {
        get => isChecked;
        set
        {
            SetProperty(ref isChecked, value);
            notifyCommands?.Invoke();
        }
    }

    public abstract string Name { get; set; }

    public RelayCommand SettingsCommand { get; }

    protected ISettingsDialogService SettingsDialogService { get; }

    public abstract void ProcessSegmentPair(ISegmentPair segmentPair);

    protected abstract void OnSettingsActivated();

    protected virtual bool SettingsCanExecute()
    {
        return true;
    }
}
