using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.Exceptions;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.TagTypes;
using Leo.SdlxliffEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Leo.SdlxliffEditor.Views;

public partial class SegmentPairView : UserControl
{
    public static readonly DependencyProperty IsSourceChangedProperty =
        DependencyProperty.Register("IsSourceChanged", typeof(bool), typeof(SegmentPairView),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty IsTargetChangedProperty =
        DependencyProperty.Register("IsTargetChanged", typeof(bool), typeof(SegmentPairView),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public SegmentPairView()
    {
        InitializeComponent();
    }

    public bool IsSourceChanged
    {
        get => (bool)GetValue(IsSourceChangedProperty);
        set => SetValue(IsSourceChangedProperty, value);
    }

    public bool IsTargetChanged
    {
        get => (bool)GetValue(IsTargetChangedProperty);
        set => SetValue(IsTargetChangedProperty, value);
    }

    private static TextPointer GetEndLine(TextPointer caretPosition)
    {
        var index = 1;
        var endLine = caretPosition.GetLineStartPosition(0);
        TextPointer nextLine;

        do
        {
            nextLine = caretPosition.GetLineStartPosition(index);

            if (nextLine != null)
            {
                endLine = nextLine;
                index++;
            }
        } while (nextLine != null);

        return endLine;
    }

    private static TextPointer GetStartLine(TextPointer caretPosition)
    {
        var index = -1;
        var startLine = caretPosition.GetLineStartPosition(0);
        TextPointer previousLine;

        do
        {
            previousLine = caretPosition.GetLineStartPosition(index);

            if (previousLine != null)
            {
                startLine = previousLine;
                index--;
            }
        } while (previousLine != null);

        return startLine;
    }

    private static bool IsCaretOnEndLine(TextPointer caretPosition)
    {
        // Current line that the caret is on
        var currentLine = caretPosition.GetLineStartPosition(0);
        var endLine = GetEndLine(caretPosition);

        return currentLine.CompareTo(endLine) == 0;
    }

    private static bool IsCaretOnStartLine(TextPointer caretPosition)
    {
        // Current line that the caret is on
        var currentLine = caretPosition.GetLineStartPosition(0);
        var startLine = GetStartLine(caretPosition);

        return currentLine.CompareTo(startLine) == 0;
    }

    private static void MoveSegmentFocus(KeyEventArgs e, RichTextBox rtb, bool moveDown = true)
    {
        var listbox = ElementLookupHelper.FindParent<ListBox>(rtb);

        // Make sure selected index is greater than zero
        if (listbox.SelectedItems.Count == 1)
        {
            listbox.SelectedIndex += moveDown ? 1 : -1;

            // ContainerFromIndex throws System.IndexOutOfRangeException if selected index is negative
            if (listbox.SelectedIndex >= 0)
            {
                var listBoxItem = listbox.ItemContainerGenerator.ContainerFromIndex(listbox.SelectedIndex);
                var target = ElementLookupHelper.GetLastChildOfType<RichTextBox>(listBoxItem);
                target.Focus();
                e.Handled = true;
            }
        }
    }

    private static void SelectRow(RichTextBox rtb)
    {
        // Clear all previous selections
        var listBox = ElementLookupHelper.FindParent<ListBox>(rtb);
        listBox.UnselectAll();

        // Select listbox item that has focus
        var listBoxItem = ElementLookupHelper.FindParent<ListBoxItem>(rtb);
        listBoxItem.IsSelected = true;
    }

    private static void UnSelectRow(RichTextBox rtb)
    {
        var parent = ElementLookupHelper.FindParent<ListBoxItem>(rtb);

        // Only set to false if a child of this list box item does not have keyboard focus
        if (!parent.IsKeyboardFocusWithin)
        {
            parent.IsSelected = false;
        }
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (DataContext is ISegmentPairViewModel segmentPairViewModel)
            {
                bool isSourceChange = false;
                bool isTargetChange = false;

                foreach (CommentGroupViewModel group in e.OldItems)
                {
                    isSourceChange |= group.IsSource;
                    isTargetChange |= !group.IsSource;
                }

                if (isSourceChange)
                {
                    RichTextBoxHelper.UpdateFlowDocument(Source, segmentPairViewModel.SourceSegment);
                }

                if (isTargetChange)
                {
                    RichTextBoxHelper.UpdateFlowDocument(Target, segmentPairViewModel.TargetSegment);
                }

                // Update the comments binding
                segmentPairViewModel.HasComments = segmentPairViewModel.Comments.Count > 0;
            }
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ISegmentPairViewModel segmentPairViewModel)
        {
            segmentPairViewModel.Comments.CollectionChanged += OnCollectionChanged;
        }
    }

    private void OnPreviewKeyUp(object sender, KeyEventArgs e)
    {
        if ((e.Key == Key.Back || e.Key == Key.Delete) && sender is RichTextBox rtb)
        {
            Stack<Inline> stack = new();
            List<Inline> toRemove = new();

            foreach (Paragraph p in rtb.Document?.Blocks)
            {
                foreach (var inline in p.Inlines.Where(i => i is InlineUIContainer { Tag: TagPairTagType or null }))
                {
                    if (inline.Tag is null)
                    {
                        if (stack.Count > 0)
                        {
                            // If something is on the stack,
                            // we just pop it off and discard it as we found its corresponding end
                            _ = stack.Pop();
                        }
                        else
                        {
                            // If the stack is empty we
                            // hit an end tag with no corresponding begin tag
                            toRemove.Add(inline);
                        }
                    }
                    else
                    {
                        stack.Push(inline);
                    }
                }
            }

            var paragraph = rtb.Document?.Blocks.FirstOrDefault() as Paragraph;

            if (paragraph is not null)
            {
                while (stack.Count > 0)
                {
                    paragraph.Inlines.Remove(stack.Pop());
                }

                foreach (var inline in toRemove)
                {
                    paragraph.Inlines.Remove(inline);
                }
            }
        }
    }

    private void OnSourceGotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            rtb.TextChanged += OnSourceTextChanged;
            rtb.PreviewKeyDown += OnSourcePreviewKeyDown;
            rtb.PreviewKeyUp += OnPreviewKeyUp;

            SelectRow(rtb);
        }
    }

    private void OnSourceLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            rtb.TextChanged -= OnSourceTextChanged;
            rtb.PreviewKeyDown -= OnSourcePreviewKeyDown;
            rtb.PreviewKeyUp -= OnPreviewKeyUp;

            UnSelectRow(rtb);

            if (rtb.Tag is ISegmentPairViewModel segmentPair)
            {
                if (IsSourceChanged)
                {
                    UpdateSegment(rtb.Document, segmentPair.SourceSegment, segmentPair.Document, segmentPair.RowNumber);
                }

                rtb.Tag = null;
            }
        }
    }

    private void OnSourcePreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            if (e.Key == Key.Down)
            {
                if (IsCaretOnEndLine(rtb.CaretPosition))
                {
                    rtb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (IsCaretOnStartLine(rtb.CaretPosition))
                {
                    MoveSegmentFocus(e, rtb, false);
                }
            }
        }
    }

    private void OnSourcePreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        UpdateContextMenuCanExecute();
    }

    private void OnSourceTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            IsSourceChanged = true;
            rtb.Tag = rtb.DataContext;
        }
    }

    private void OnTargetGotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            rtb.TextChanged += OnTargetTextChanged;
            rtb.PreviewKeyDown += OnTargetPreviewKeyDown;
            rtb.PreviewKeyUp += OnPreviewKeyUp;

            SelectRow(rtb);
        }
    }

    private void OnTargetLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            rtb.TextChanged -= OnTargetTextChanged;
            rtb.PreviewKeyDown -= OnTargetPreviewKeyDown;
            rtb.PreviewKeyUp -= OnPreviewKeyUp;

            UnSelectRow(rtb);

            if (rtb.Tag is ISegmentPairViewModel segmentPair)
            {
                if (IsTargetChanged)
                {
                    UpdateSegment(rtb.Document, segmentPair.TargetSegment, segmentPair.Document, segmentPair.RowNumber, false);
                }

                rtb.Tag = null;
            }
        }
    }

    private void OnTargetPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            if (e.Key == Key.Down)
            {
                if (IsCaretOnEndLine(rtb.CaretPosition))
                {
                    MoveSegmentFocus(e, rtb);
                }
            }
            else if (e.Key == Key.Up)
            {
                if (IsCaretOnStartLine(rtb.CaretPosition))
                {
                    rtb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    e.Handled = true;
                }
            }
        }
    }

    private void OnTargetPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        UpdateContextMenuCanExecute();
    }

    private void OnTargetTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is RichTextBox rtb)
        {
            IsTargetChanged = true;
            rtb.Tag = rtb.DataContext;
        }
    }

    private void UpdateContextMenuCanExecute()
    {
        if (DataContext is ISegmentPairViewModel segmentPairViewModel)
        {
            segmentPairViewModel.UpdateCanExecute();
        }
    }

    private void UpdateSegment(FlowDocument document, ISegment segment, ISdlxliffDocument sdlxliffDocument, int rowNumber, bool isSource = true)
    {
        try
        {
            SegmentConverter.Update(document, segment, sdlxliffDocument);
        }
        catch (Exception ex) when (ex is MissingTagException or AggregateException)
        {
            ex.Data.Add("RowNumber", rowNumber);
            MessageBoxHelper.ShowErrorMessage(ex);
        }
        finally
        {
            if (isSource)
            {
                IsSourceChanged = false;
            }
            else
            {
                IsTargetChanged = false;
            }
        }
    }
}