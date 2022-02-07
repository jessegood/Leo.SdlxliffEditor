using System.Windows;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.Dialogs.Views;

/// <summary>
/// Interaction logic for LoadQAFilesDialog.xaml
/// </summary>
public partial class LoadFilesDialogView : Window
{
    public LoadFilesDialogView()
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
