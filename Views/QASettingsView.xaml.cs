using Leo.SdlxliffEditor.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace Leo.SdlxliffEditor.Views;

/// <summary>
/// Interaction logic for QASettingsView.xaml
/// </summary>
public partial class QASettingsView : UserControl
{
    public QASettingsView()
    {
        InitializeComponent();
    }

    private void OnCheckBoxClick(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            var listBox = ElementLookupHelper.FindParent<ListBox>(checkBox);
            listBox.SelectedItem = checkBox.DataContext;
        }
    }
}