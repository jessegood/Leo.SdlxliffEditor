using Microsoft.Toolkit.Mvvm.Input;

namespace Leo.SdlxliffEditor.Interfaces;

public interface IFindAndReplaceDialogViewModel
{
    RelayCommand CloseCommand { get; }
    RelayCommand FindNextCommand { get; }
    string FindText { get; set; }
    bool IsCaseSensitive { get; set; }
    RelayCommand ReplaceAllCommand { get; }
    RelayCommand ReplaceCommand { get; }
    string ReplaceText { get; set; }
    int SelectedIndex { get; set; }
    bool UseRegularExpressions { get; set; }
    void Show(IEditorViewModel editorViewModel);
}