using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Windows.Input;

namespace Leo.SdlxliffEditor.ViewModels;

public class MainWindowViewModel : ObservableRecipient
{
    private readonly ISdlxliffFileService sdlxliffFileService;
    private readonly IServiceProvider serviceProvider;
    private ObservableObject currentViewModel;
    private bool isLoading;

    public MainWindowViewModel(IServiceProvider serviceProvider, ISdlxliffFileService sdlxliffFileService, IMessenger messenger)
        : base(messenger)
    {
        this.serviceProvider = serviceProvider;
        this.sdlxliffFileService = sdlxliffFileService;

        // Navigation commands
        NavigateEditorViewCommand = new RelayCommand(NavigateEditorView, CanExecuteNavigateEditorViewCommand);

        NavigateQAViewCommand = new RelayCommand(NavigateQAView, CanExecuteQAViewCommand);

        NavigateBatchTaskViewCommand = new RelayCommand(NavigateBatchTaskView, CanExecuteBatchTaskViewCommand);

        NavigateSettingsViewCommand = new RelayCommand(NavigateSettingsView, CanExecuteSettingsViewCommand);

        LoadedCommand = new RelayCommand(OnLoaded);
    }

    public ObservableObject CurrentViewModel
    {
        get => currentViewModel;
        set => SetProperty(ref currentViewModel, value);
    }

    public bool IsLoading
    {
        get => isLoading;
        set => SetProperty(ref isLoading, value);
    }

    public ICommand LoadedCommand { get; }

    public RelayCommand NavigateBatchTaskViewCommand { get; }

    public RelayCommand NavigateEditorViewCommand { get; }

    public RelayCommand NavigateQAViewCommand { get; }

    public RelayCommand NavigateSettingsViewCommand { get; }

    protected override void OnActivated()
    {
        Messenger.Register<MainWindowViewModel, EditorFinishedLoadingRequestMessage>(this, (mw, m) =>
        {
            IsLoading = false;
        });

        Messenger.Register<MainWindowViewModel, EditorCloseRequestMessage>(this, (mw, m) =>
        {
            IsActive = false;
            IsLoading = false;
            sdlxliffFileService.ClearFiles();
            CurrentViewModel = CreateEditorLayoutViewModel(serviceProvider.GetRequiredService<DragAndDropFileViewModel>());
        });
    }

    protected override void OnDeactivated()
    {
        Messenger.Unregister<EditorFinishedLoadingRequestMessage>(this);
        Messenger.Unregister<EditorCloseRequestMessage>(this);
    }

    private bool CanExecuteBatchTaskViewCommand()
    {
        return CurrentViewModel is not BatchTaskViewModel;
    }

    private bool CanExecuteNavigateEditorViewCommand()
    {
        return CurrentViewModel is not null and not EditorLayoutViewModel;
    }

    private bool CanExecuteQAViewCommand()
    {
        return CurrentViewModel is not QAViewModel;
    }

    private bool CanExecuteSettingsViewCommand()
    {
        return CurrentViewModel is not SettingsViewModel;
    }

    private EditorLayoutViewModel CreateEditorLayoutViewModel(ObservableObject editorContent)
    {
        return new(editorContent, serviceProvider.GetRequiredService<ISdlxliffFileService>(),
            serviceProvider.GetRequiredService<ILoadFilesDialogViewModel>(),
            serviceProvider.GetRequiredService<IMessenger>(),
            serviceProvider.GetRequiredService<IMessageBoxService>(),
            serviceProvider.GetRequiredService<ICommandExceptionHandler>());
    }

    private void NavigateBatchTaskView()
    {
        IsActive = false;

        CurrentViewModel = serviceProvider.GetRequiredService<BatchTaskViewModel>();
        NotifyCanExecuteChanged();
    }
    private void NavigateDragAndDropFileView()
    {
        CurrentViewModel = CreateEditorLayoutViewModel(serviceProvider.GetRequiredService<DragAndDropFileViewModel>());
        NotifyCanExecuteChanged();
    }

    private void NavigateEditorView()
    {
        // Only navigate to editor view if the file service has files stored
        if (sdlxliffFileService.HasFiles)
        {
            IsActive = true;
            IsLoading = true;
            CurrentViewModel = CreateEditorLayoutViewModel(serviceProvider.GetRequiredService<EditorViewModel>());
        }
        else
        {
            NavigateDragAndDropFileView();
        }

        NotifyCanExecuteChanged();
    }

    private void NavigateQAView()
    {
        IsActive = false;

        CurrentViewModel = serviceProvider.GetRequiredService<QAViewModel>();
        NotifyCanExecuteChanged();
    }

    private void NavigateSettingsView()
    {
        IsActive = false;

        CurrentViewModel = serviceProvider.GetRequiredService<SettingsViewModel>();
        NotifyCanExecuteChanged();
    }

    private void NotifyCanExecuteChanged()
    {
        NavigateEditorViewCommand.NotifyCanExecuteChanged();
        NavigateQAViewCommand.NotifyCanExecuteChanged();
        NavigateBatchTaskViewCommand.NotifyCanExecuteChanged();
        NavigateSettingsViewCommand.NotifyCanExecuteChanged();
    }

    private void OnFilesLoaded()
    {
        // Only navigate if we are on the editor layout view
        if (CurrentViewModel is EditorLayoutViewModel)
        {
            NavigateEditorView();
        }
    }

    private void OnLoaded()
    {
        IsActive = false;
        NavigateDragAndDropFileView();
        sdlxliffFileService.OnFilesLoaded += OnFilesLoaded;
    }
}
