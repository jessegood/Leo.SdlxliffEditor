using AdonisUI;
using Leo.SdlxliffEditor.Interfaces;

namespace Leo.SdlxliffEditor.Services;

public class ColorSchemeService : IColorSchemeService
{
    private const string IsDark = "IsDark";

    public bool GetColorScheme()
    {
        if (App.Current.Properties.Contains(IsDark))
        {
            return (bool)App.Current.Properties[IsDark];
        }

        // Default to the light color scheme
        return false;
    }

    public void SetColorScheme(bool isDark)
    {
        if (App.Current.Properties.Contains(IsDark))
        {
            App.Current.Properties[IsDark] = isDark;
        }
        else
        {
            App.Current.Properties.Add(IsDark, isDark);
        }

        ResourceLocator.SetColorScheme(App.Current.Resources, isDark ? ResourceLocator.DarkColorScheme : ResourceLocator.LightColorScheme);
    }
}
