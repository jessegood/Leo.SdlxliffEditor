using System.Windows;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.Dialogs.Views;

/// <summary>
/// Interaction logic for SettingsDialogView.xaml
/// </summary>
public partial class SettingsDialogView : Window
{
    public SettingsDialogView()
    {
        InitializeComponent();
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;

        foreach (var bindingExpression in BindingOperations.GetSourceUpdatingBindings(this))
        {
            bindingExpression.UpdateSource();
        }
    }
}
