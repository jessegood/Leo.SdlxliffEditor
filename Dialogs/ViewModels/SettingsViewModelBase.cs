using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Leo.SdlxliffEditor.Dialogs.ViewModels;

public abstract class SettingsViewModelBase : ObservableObject
{
    public abstract string Title { get; set; }
}
