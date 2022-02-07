using Leo.SdlxliffEditor.Dialogs.Views;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Leo.SdlxliffEditor.Services;

public class SettingsDialogService : ISettingsDialogService
{
    private readonly MainWindow mainWindow;

    public SettingsDialogService(MainWindow mainWindow)
    {
        this.mainWindow = mainWindow;
    }

    public SettingsDialogView CreateSettingsDialog(ObservableObject viewModel)
    {
        SettingsDialogView settingsDialogView = new();
        settingsDialogView.Owner = mainWindow;
        settingsDialogView.DataContext = viewModel;

        return settingsDialogView;
    }
}
