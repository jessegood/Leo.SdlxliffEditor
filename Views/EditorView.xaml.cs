using Leo.SdlxliffEditor.Dialogs.ViewModels;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Leo.SdlxliffEditor.Views;

public partial class EditorView : UserControl
{
    public EditorView()
    {
        InitializeComponent();
    }

    private void HighlightFoundText(MatchInfo matchInfo)
    {
        SetInactiveSelection();
        ScrollToIndex(matchInfo.RowNumber);
        SegmentList.UpdateLayout();

        var listBoxItem = (ListBoxItem)SegmentList.ItemContainerGenerator.ContainerFromIndex(matchInfo.RowNumber);
        var richTextBoxes = ElementLookupHelper.GetChildrenOfType<RichTextBox>(listBoxItem);

        HighlightText(matchInfo.IsSource ? richTextBoxes.First() : richTextBoxes.Last(), matchInfo.Match, matchInfo.Occurrence);
    }

    private static void HighlightText(RichTextBox rtb, string searchText, int occurrence)
    {
        int count = 0;

        for (TextPointer startPointer = rtb.Document.ContentStart;
             startPointer.CompareTo(rtb.Document.ContentEnd) <= 0;
             startPointer = startPointer.GetNextContextPosition(LogicalDirection.Forward))
        {
            // Check if we are at the end
            if (startPointer.CompareTo(rtb.Document.ContentEnd) == 0)
            {
                break;
            }

            // Get the adjacent string
            string adjacentString = startPointer.GetTextInRun(LogicalDirection.Forward);

            // Check if the search string is present
            int index = adjacentString.IndexOf(searchText);

            // As long as the index is greater than -1
            // we loop to find all occurrences in current symbol
            while (index > -1)
            {
                // Keep track of the number of found instances
                count++;

                // Set the start pointer at the index position
                startPointer = startPointer.GetPositionAtOffset(index);

                // End pointer will be the length of the search string
                TextPointer endPointer = startPointer.GetPositionAtOffset(searchText.Length);

                if (count == occurrence)
                {
                    // Display selected text even if this rich text box does not have focus
                    rtb.IsInactiveSelectionHighlightEnabled = true;

                    // Set focus to richtextbox
                    rtb.Focus();

                    // Select the text
                    rtb.Selection.Select(startPointer, endPointer);
                    return;
                }


                // Set the start to the end of the current match
                startPointer = endPointer;

                // Get the adjacent string from the end position
                adjacentString = startPointer.GetTextInRun(LogicalDirection.Forward);

                // Check if the search string is present
                index = adjacentString.IndexOf(searchText);
            }
        }
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        if (sender is EditorView editorView)
        {
            var dataContext = editorView.DataContext as IEditorViewModel;
            dataContext.FileSelected += ScrollToIndex;
            dataContext.FindTextExecuted += HighlightFoundText;
            dataContext.ReplaceTextExecuted += ReplaceFoundText;

            // Update the default highlight brush for find and replace
            SegmentList.Resources[SystemColors.InactiveSelectionHighlightBrushKey] = SystemColors.HighlightBrush;
        }
    }

    private void ReplaceFoundText(MatchInfo matchInfo, string replaceText)
    {
        var listBoxItem = (ListBoxItem)SegmentList.ItemContainerGenerator.ContainerFromIndex(matchInfo.RowNumber);
        var richTextBoxes = ElementLookupHelper.GetChildrenOfType<RichTextBox>(listBoxItem);

        var rtb = matchInfo.IsSource ? richTextBoxes.First() : richTextBoxes.Last();
        if (!rtb.Selection.IsEmpty)
        {
            rtb.Selection.Text = replaceText;
        }
    }


    private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        // Change the title only when there vertical change is not zero
        if (e.VerticalChange != 0 && sender is ListBox listBox)
        {
            var dataContent = listBox.DataContext as IEditorViewModel;
            dataContent.UpdateTitle(e.VerticalOffset);
        }
    }

    private void ScrollToIndex(int index)
    {
        var scrollViewer = ElementLookupHelper.GetChildOfType<ScrollViewer>(SegmentList);
        if (scrollViewer is not null)
        {
            scrollViewer.ScrollToVerticalOffset(index);
        }
    }

    private void SetInactiveSelection()
    {
        var scrollViewer = ElementLookupHelper.GetChildOfType<ScrollViewer>(SegmentList);
        if (scrollViewer is not null)
        {
            var startIndex = (int)scrollViewer.VerticalOffset;
            var itemCount = startIndex + (int)scrollViewer.ViewportHeight;
            for (int i = startIndex; i <= itemCount; i++)
            {
                var listBoxItem = SegmentList.ItemContainerGenerator.ContainerFromIndex(i);
                if (listBoxItem is not null)
                {
                    var richTextBoxes = ElementLookupHelper.GetChildrenOfType<RichTextBox>(listBoxItem);
                    foreach (var rtb in richTextBoxes)
                    {
                        rtb.IsInactiveSelectionHighlightEnabled = false;
                    }
                }
            }
        }
    }
}