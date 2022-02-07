using AdonisUI.Controls;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.Views;

namespace Leo.SdlxliffEditor.Services;

public class MessageBoxService : IMessageBoxService
{
    private readonly MainWindow mainWindow;

    public MessageBoxService(MainWindow mainWindow)
    {
        this.mainWindow = mainWindow;
    }

    public MessageBoxResult ShowUnsavedChangesMessageBox()
    {
        return MessageBoxHelper.ShowConfirmationMessage(mainWindow, (string)App.Current.Resources["UnsavedChanges"], MessageBoxButton.YesNo);
    }
}