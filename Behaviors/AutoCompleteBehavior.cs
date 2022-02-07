using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Leo.SdlxliffEditor.Behaviors;

public sealed class AutoCompleteBehavior : Behavior<RichTextBox>
{
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable<object>), typeof(AutoCompleteBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnItemsSourceChanged));

    private readonly ListBox listBox = new ListBox();

    private readonly Popup popup = new Popup();

    public AutoCompleteBehavior()
    {
        AutoCompleteCommand = new RelayCommand(TogglePopup, () => listBox.ItemsSource is not null);

        listBox.Height = 100;
        listBox.ItemContainerStyle = (Style)App.Current.FindResource("AutoCompleteStyle");
        listBox.PreviewKeyDown += OnListBoxPreviewKeyDown;
        listBox.BorderThickness = new Thickness(1);
        listBox.BorderBrush = (Brush)App.Current.FindResource(AdonisUI.Brushes.Layer1BorderBrush);
        listBox.Background = (Brush)App.Current.FindResource(AdonisUI.Brushes.Layer0BackgroundBrush);

        popup.Child = listBox;
        popup.StaysOpen = false;
        popup.Placement = PlacementMode.Custom;
        popup.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(OnPlacePopup);
    }

    public RelayCommand AutoCompleteCommand { get; }

    public IEnumerable<object> ItemsSource
    {
        get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public void SetAutoCompleteItemsSource(IEnumerable<object> itemsSource)
    {
        listBox.ItemsSource = itemsSource;
        AutoCompleteCommand.NotifyCanExecuteChanged();
    }

    protected override void OnAttached()
    {
        var inputBinding = new InputBinding(AutoCompleteCommand,
            new KeyGesture(Key.OemComma, ModifierKeys.Control));
        AssociatedObject.InputBindings.Add(inputBinding);

        // AssociatedObject needs to be set after behavior is attached
        popup.PlacementTarget = AssociatedObject;
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AutoCompleteBehavior behavior && e.NewValue is not null)
        {
            behavior.SetAutoCompleteItemsSource((IEnumerable<object>)e.NewValue);
        }
    }

    private void ClosePopup()
    {
        popup.IsOpen = false;
        AssociatedObject.Focus();
    }

    private UIElement CopyChild(UIElement child)
    {
        var border = (Border)child;
        var textBlock = (TextBlock)border.Child;

        return new Border()
        {
            Child = CopyTextBlock(textBlock),
            Background = border.Background.Clone(),
        };
    }

    private Inline CopyInline(Inline inline) =>
        inline switch
        {
            InlineUIContainer inlineUIContainer =>
                new InlineUIContainer(CopyChild(inlineUIContainer.Child))
                {
                    Tag = inlineUIContainer.Tag,
                    BaselineAlignment = BaselineAlignment.Bottom
                },
            Run run => new Run(run.Text),
            _ => throw new ArgumentException($"Unknown inline found {inline.GetType()}")
        };

    private UIElement CopyTextBlock(TextBlock textBlock)
    {
        TextBlock copy = new TextBlock();

        foreach (var inline in textBlock.Inlines)
        {
            copy.Inlines.Add(CopyInline(inline));
        }

        return copy;
    }

    private void InsertSelection()
    {
        if (listBox.SelectedItem is string s)
        {
            AssociatedObject.CaretPosition.InsertTextInRun((string)s);
        }
        else if (listBox.SelectedItem is TextBlock textBlock)
        {
            InlineUIContainer inlineUIContainer = null;
            foreach (var inline in textBlock.Inlines)
            {
                if (inline is InlineUIContainer container)
                {
                    inlineUIContainer = new InlineUIContainer(CopyChild(container.Child), GetInsertionPosition(inlineUIContainer))
                    {
                        Tag = container.Tag,
                        BaselineAlignment = BaselineAlignment.Bottom
                    };
                }
            }
        }

        // The first time we just insert at the current caret position
        // The second time onwards, we insert after the previously inserted container
        TextPointer GetInsertionPosition(InlineUIContainer inlineUIContainer)
            => inlineUIContainer is null ? AssociatedObject.CaretPosition : inlineUIContainer.ContentEnd;
    }

    private void OnListBoxPreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Tab:
            case Key.Enter:
            case Key.Right:
                // Close popup and insert selection
                ClosePopup();
                InsertSelection();
                e.Handled = true;
                break;

            case Key.Up:
            case Key.Down:
                // Do nothing
                break;

            default:
                // Close popup and give focus back to richtextbox
                ClosePopup();
                e.Handled = true;
                break;
        }
    }

    private CustomPopupPlacement[] OnPlacePopup(Size popupSize, Size targetSize, Point offset)
    {
        var xOffset = AssociatedObject.CaretPosition.GetCharacterRect(LogicalDirection.Forward).Right;
        var yOffset = AssociatedObject.CaretPosition.GetCharacterRect(LogicalDirection.Forward).Bottom;

        var placementOne = new CustomPopupPlacement(new Point(xOffset + 20, yOffset + 5), PopupPrimaryAxis.Horizontal);
        var placementTwo = new CustomPopupPlacement(new Point(xOffset + 40, yOffset + 10), PopupPrimaryAxis.Horizontal);

        return new[] { placementOne, placementTwo };
    }

    private void TogglePopup()
    {
        if (popup.IsOpen)
        {
            ClosePopup();
        }
        else
        {
            popup.IsOpen = true;
            if (listBox.SelectedItem is not null)
            {
                var listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem);
                listBoxItem.Focus();
            }
            else
            {
                listBox.Focus();
            }
        }
    }
}