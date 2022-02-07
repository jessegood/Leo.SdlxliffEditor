using Leo.SdlxliffEditor.Dialogs.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Leo.SdlxliffEditor.Interfaces;

public interface ISettingsDialogService
{
    SettingsDialogView CreateSettingsDialog(ObservableObject viewModel);
}
