using Leo.SdlxliffEditor.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Leo.SdlxliffEditor.Views;

/// <summary>
/// Interaction logic for EditorLayoutView.xaml
/// </summary>
public partial class EditorLayoutView : UserControl
{
    public EditorLayoutView()
    {
        InitializeComponent();

        // Set focus on this control so keybindings work
        Focusable = true;
        Loaded += (s, e) => Keyboard.Focus(this);
    }

    private void OnDecreaseFont(object sender, RoutedEventArgs e)
    {
        var listBox = ElementLookupHelper.GetChildOfType<ListBox>(Editor);
        if (listBox != null && listBox.FontSize > 0)
        {
            listBox.FontSize -= 1;
        }
    }

    private void OnIncreaseFont(object sender, RoutedEventArgs e)
    {
        var listBox = ElementLookupHelper.GetChildOfType<ListBox>(Editor);
        if (listBox != null)
        {
            listBox.FontSize += 1;
        }
    }

    private void OnToggleTagDisplay(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton tb)
        {
            if (tb.IsChecked is not null)
            {
                foreach (var rtb in ElementLookupHelper.GetChildrenOfType<RichTextBox>(Editor))
                {
                    RichTextBoxHelper.ChangeTagDisplay(rtb, tb.IsChecked == true);
                }
            }
        }
    }
}