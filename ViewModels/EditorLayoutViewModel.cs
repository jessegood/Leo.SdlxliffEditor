using AdonisUI.Controls;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Leo.SdlxliffEditor.ViewModels;

public sealed class EditorLayoutViewModel : ObservableObject
{
    private readonly ILoadFilesDialogViewModel loadFilesDialogViewModel;
    private readonly IMessageBoxService messageBoxService;
    private readonly IMessenger messenger;
    private readonly ISdlxliffFileService sdlxliffFileService;
    private string title;

    public EditorLayoutViewModel(ObservableObject editorContentViewModel, ISdlxliffFileService sdlxliffFileService,
        ILoadFilesDialogViewModel loadFilesDialogViewModel, IMessenger messenger, IMessageBoxService messageBoxService,
        ICommandExceptionHandler commandExceptionHandler)
    {
        EditorContentViewModel = editorContentViewModel;
        this.sdlxliffFileService = sdlxliffFileService;
        this.loadFilesDialogViewModel = loadFilesDialogViewModel;
        this.messenger = messenger;
        this.messageBoxService = messageBoxService;
        CloseCommand = new AsyncRelayCommand(OnClose);
        OpenCommand = new RelayCommand(OnOpen);
        SaveCommand = new AsyncRelayCommand(OnSave);
        FindAndReplaceCommand = new RelayCommand(OnFindAndReplace);

        commandExceptionHandler.RegisterCommands(CloseCommand, SaveCommand);

        SetTitle();
    }

    public IAsyncRelayCommand CloseCommand { get; }

    public ObservableObject EditorContentViewModel { get; }

    public IRelayCommand FindAndReplaceCommand { get; }

    public IRelayCommand OpenCommand { get; }

    public IAsyncRelayCommand SaveCommand { get; }

    public string Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }

    private async Task Close(IEditorViewModel editorViewModel)
    {
        var segmentPairs = (ObservableCollection<ISegmentPairViewModel>)editorViewModel.SegmentPairsView.Source;

        if (editorViewModel.HasUnsavedChanges ||
            segmentPairs.Any(sp => sp.IsChanged))
        {
            var result = messageBoxService.ShowUnsavedChangesMessageBox();
            if (result == MessageBoxResult.No)
            {
                editorViewModel.Close();
            }
            else if (result == MessageBoxResult.Yes)
            {
                await OnSave();
                editorViewModel.Close();
            }
        }
        else
        {
            editorViewModel.Close();
        }
    }

    private async Task OnClose()
    {
        if (EditorContentViewModel is IEditorViewModel editorViewModel)
        {
            editorViewModel.FileChanged -= OnFileChanged;
            await Close(editorViewModel);
            messenger.Send<EditorCloseRequestMessage>();
        }
    }

    private void OnFileChanged(string fileName)
    {
        Title = fileName;
    }

    private void OnFindAndReplace()
    {
        if (EditorContentViewModel is IEditorViewModel editorViewModel)
        {
            editorViewModel.Show();
        }
    }

    private void OnOpen()
    {
        if (sdlxliffFileService.HasFiles)
        {
            loadFilesDialogViewModel.LoadStoredFiles(sdlxliffFileService.FileList);
        }

        if (loadFilesDialogViewModel.ShowDialog() == true)
        {
            if (loadFilesDialogViewModel.HasFiles)
            {
                sdlxliffFileService.LoadFiles(loadFilesDialogViewModel.FilePaths);
            }
        }
    }

    private async Task OnSave()
    {
        if (EditorContentViewModel is IEditorViewModel editorViewModel)
        {
            await editorViewModel.Save();
            editorViewModel.HasUnsavedChanges = false;
            editorViewModel.ResetChangeTracker();
        }
    }

    private void SetTitle()
    {
        if (EditorContentViewModel is IEditorViewModel editorViewModel)
        {
            editorViewModel.FileChanged += OnFileChanged;
        }
    }
}