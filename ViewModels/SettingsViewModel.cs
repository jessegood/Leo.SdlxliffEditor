using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Leo.SdlxliffEditor.ViewModels;

public class SettingsViewModel : ObservableObject
{
    private readonly IColorSchemeService colorSchemeService;
    private FontFamily fontFamily;
    private bool isDark;

    public SettingsViewModel(IColorSchemeService colorSchemeService)
    {
        this.colorSchemeService = colorSchemeService;
        LoadedCommand = new RelayCommand(OnLoaded);
    }

    public FontFamily FontFamily
    {
        get => fontFamily;
        set => SetProperty(ref fontFamily, value);
    }

    public bool IsDark
    {
        get => isDark;
        set => SetProperty(ref isDark, value);
    }

    public ICommand LoadedCommand { get; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        colorSchemeService.SetColorScheme(IsDark);
    }

    private void OnLoaded()
    {
        IsDark = colorSchemeService.GetColorScheme();
    }
}
