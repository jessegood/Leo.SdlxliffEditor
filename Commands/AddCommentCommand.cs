using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Leo.SdlxliffEditor.Commands;

public class AddCommentCommand : BaseCommand
{
    private readonly ISegmentPairViewModel segmentPairViewModel;

    public AddCommentCommand(ISegmentPairViewModel segmentPairViewModel)
    {
        this.segmentPairViewModel = segmentPairViewModel;
    }

    public override bool CanExecute(object parameter)
    {
        if (parameter is ListBox listBox)
        {
            var isEmpty = true;

            var listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(segmentPairViewModel);

            foreach (var richTextBox in ElementLookupHelper.GetChildrenOfType<RichTextBox>(listBoxItem))
            {
                isEmpty &= richTextBox.Selection.IsEmpty;
            }

            return !isEmpty;
        }

        return false;
    }

    public override void Execute(object parameter)
    {
        if (parameter is ListBox listBox)
        {
            var listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(segmentPairViewModel);

            foreach (var richTextBox in ElementLookupHelper.GetChildrenOfType<RichTextBox>(listBoxItem))
            {
                if (!richTextBox.Selection.IsEmpty)
                {
                    // A text selection can be already yellow highlighted text as well
                    // so we skip those
                    if (!(richTextBox.Selection.Start.Parent is Run r && r.Background == Brushes.Yellow))
                    {
                        richTextBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
                        CreateNewComment(richTextBox);
                    }
                }
            }
        }
    }

    private static IEnumerable<Run> GetCommentRuns(RichTextBox richTextBox)
    {
        foreach (var block in richTextBox.Document.Blocks)
        {
            if (block is Paragraph p)
            {
                foreach (var inline in p.Inlines)
                {
                    if (inline is Run r && r.Background == Brushes.Yellow && r.Tag == null)
                    {
                        yield return r;
                    }
                }
            }
        }
    }

    private void CreateNewComment(RichTextBox richTextBox)
    {
        var commentRuns = GetCommentRuns(richTextBox);
        var commentMarker = new CommentMarker()
        {
            Parent = GetParent(commentRuns.First(), richTextBox),
            Id = Guid.NewGuid().ToString()
        };

        commentMarker.CommentDefinition.Comments.Add(new Comment(string.Empty)
        {
            Date = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture),
            User = string.Empty,
            Version = "1.0",
            Severity = CommentSeverity.Low
        });

        foreach (var run in commentRuns)
        {
            run.Tag = commentMarker;
        }

        if (richTextBox.Name.Equals("Source", StringComparison.InvariantCulture))
        {
            segmentPairViewModel.Comments.Add(new CommentGroupViewModel(1, commentMarker, segmentPairViewModel, true));
        }
        else
        {
            segmentPairViewModel.Comments.Add(new CommentGroupViewModel(1, commentMarker, segmentPairViewModel));
        }

        segmentPairViewModel.Document.CommentMarkers.Add(commentMarker);
        segmentPairViewModel.HasComments = true;
    }

    private TranslationUnitContentContainer GetParent(Run commentRun, RichTextBox richTextBox)
    {
        // If the previous inline is a InlineUiContainer and its
        // Tag is a TagPair, we set that as the parent
        if (commentRun.PreviousInline is InlineUIContainer inlineUIContainer
            && inlineUIContainer.Tag is TagPair tagPair)
        {
            return tagPair;
        }
        // Otherwise, we set the source or target as the parent
        else
        {
            if (richTextBox.Name.Equals("Source", StringComparison.InvariantCulture))
            {
                return segmentPairViewModel.SourceSegment as TranslationUnitContentContainer;
            }
            else
            {
                return segmentPairViewModel.TargetSegment as TranslationUnitContentContainer;
            }
        }
    }
}
