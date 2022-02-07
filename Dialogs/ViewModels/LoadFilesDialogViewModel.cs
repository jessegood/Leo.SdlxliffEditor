using Leo.SdlxliffEditor.Dialogs.Views;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Leo.SdlxliffEditor.Dialogs.ViewModels;

public sealed class LoadFilesDialogViewModel : ObservableObject, ILoadFilesDialogViewModel
{
    private ObservableCollection<string> filePaths;

    public LoadFilesDialogViewModel()
    {
        DropCommand = new RelayCommand<object>(OnDrop);
        SelectFolderCommand = new RelayCommand(OnSelectFolder);
        SelectFileCommand = new RelayCommand(OnSelectFile);
        ClearListCommand = new RelayCommand(OnClearList, CanClearListExecute);
    }

    public event Action FileListCleared;

    public RelayCommand ClearListCommand { get; }

    public RelayCommand<object> DropCommand { get; }

    public ObservableCollection<string> FilePaths
    {
        get => filePaths;
        set => SetProperty(ref filePaths, value);
    }

    public bool HasFiles => FilePaths is not null and { Count: > 0 };

    public RelayCommand SelectFileCommand { get; set; }

    public RelayCommand SelectFolderCommand { get; }

    public void LoadStoredFiles(List<string> fileList)
    {
        FilePaths = new ObservableCollection<string>(fileList);
    }

    public bool? ShowDialog()
    {
        LoadFilesDialogView dialog = new()
        {
            DataContext = this
        };

        return dialog.ShowDialog();
    }

    private bool CanClearListExecute()
    {
        return HasFiles;
    }

    private void OnClearList()
    {
        FilePaths.Clear();
        FileListCleared?.Invoke();
        ClearListCommand.NotifyCanExecuteChanged();
    }

    private void OnDrop(object data)
    {
        FilePaths = DragAndDropHelper.GetSdlxliffs(data);
        ClearListCommand.NotifyCanExecuteChanged();
    }

    private void OnSelectFile()
    {
        FilePaths = DialogHelper.ShowOpenFileDialog();
        ClearListCommand.NotifyCanExecuteChanged();
    }

    private void OnSelectFolder()
    {
        FilePaths = DialogHelper.ShowFolderBrowserDialog();
        ClearListCommand.NotifyCanExecuteChanged();
    }
}