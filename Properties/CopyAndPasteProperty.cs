using Leo.SdlxliffEditor.TagTypes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace Leo.SdlxliffEditor.Properties;

public static class CopyAndPasteProperty
{
    public static readonly DependencyProperty CopyAndPasteBindingsProperty =
     DependencyProperty.RegisterAttached("CopyAndPasteBindings", typeof(string), typeof(CopyAndPasteProperty),
         new FrameworkPropertyMetadata(null, OnCommandBindingsChanged));

    public static string GetCopyAndPasteBindings(DependencyObject obj)
                => (string)obj.GetValue(CopyAndPasteBindingsProperty);

    public static void SetCopyAndPasteBindings(DependencyObject obj, string value)
                => obj.SetValue(CopyAndPasteBindingsProperty, value);

    private static void OnCommandBindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RichTextBox richTextBox)
        {
            richTextBox.CommandBindings.Clear();

            var copyCmdBinding = new CommandBinding(ApplicationCommands.Copy, CopyExecuted);
            var pasteCmdBinding = new CommandBinding(ApplicationCommands.Paste, PasteExecuted);

            richTextBox.CommandBindings.Add(copyCmdBinding);
            richTextBox.CommandBindings.Add(pasteCmdBinding);

            richTextBox.InputBindings.Add(new KeyBinding(ApplicationCommands.NotACommand, Key.L, ModifierKeys.Control));
            richTextBox.InputBindings.Add(new KeyBinding(ApplicationCommands.NotACommand, Key.F, ModifierKeys.Control));
        }
    }

    private static void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            var xaml = Clipboard.GetText(TextDataFormat.Text);
            if (!string.IsNullOrEmpty(xaml) && xaml.StartsWith("<"))
            {
                try
                {
                    var element = (InlineUIContainer)XamlReader.Parse(xaml);

                    // Prevent copying end elements
                    if (element.Tag is not null)
                    {
                        var child = element.Child;
                        element.Child = null;
                        CreateTag(child, rtb.CaretPosition, element.Tag);

                        // Create a tag pair for tag pair types
                        if (element.Tag is TagPairTagType)
                        {
                            var border = (Border)child;
                            var run = ((TextBlock)border.Child).Inlines.FirstInline as Run;

                            CreateTag(new Border()
                            {
                                Child = new TextBlock(new Run(run.Text.Replace("<", "</"))),
                                Background = border.Background.Clone()
                            }, rtb.CaretPosition, element.Tag);
                        }
                    }
                }
                catch (XamlParseException)
                {
                    rtb.Paste();
                }
            }
            else
            {
                rtb.Paste();
            }

            e.Handled = true;
        }
    }

    private static void CreateTag(UIElement child, TextPointer caretPosition, object tag)
    {
        new InlineUIContainer(child, caretPosition)
        {
            Tag = tag,
            BaselineAlignment = BaselineAlignment.Bottom
        };
    }

    private static void CopyExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            foreach (var block in rtb.Document.Blocks)
            {
                if (block is Paragraph p)
                {
                    foreach (var inline in p.Inlines)
                    {
                        if (rtb.Selection.Contains(inline.ContentStart) && inline is InlineUIContainer)
                        {
                            var xaml = XamlWriter.Save(inline);
                            Clipboard.SetText(xaml);
                            e.Handled = true;
                            return;
                        }
                    }
                }
            }

            Clipboard.SetText(rtb.Selection.Text);
            e.Handled = true;
        }
    }
}