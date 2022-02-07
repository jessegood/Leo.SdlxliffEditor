using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace Leo.SdlxliffEditor.ViewModels;

public class DragAndDropFileViewModel : ObservableObject
{
    private readonly ISdlxliffFileService sdlxliffFileService;

    public ICommand DropCommand { get; }

    public DragAndDropFileViewModel(ISdlxliffFileService sdlxliffFileService)
    {
        DropCommand = new RelayCommand<object>(OnDrop);
        this.sdlxliffFileService = sdlxliffFileService;
    }

    private void OnDrop(object data)
    {
        sdlxliffFileService.LoadFiles(DragAndDropHelper.GetSdlxliffs(data));
    }
}
