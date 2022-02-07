using AdonisUI.Controls;

namespace Leo.SdlxliffEditor.Interfaces;

public interface IMessageBoxService
{
    MessageBoxResult ShowUnsavedChangesMessageBox();
}