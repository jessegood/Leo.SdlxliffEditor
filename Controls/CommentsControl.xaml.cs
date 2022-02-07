using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.ViewModels;
using Leo.SdlxliffEditor.Views;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Leo.SdlxliffEditor.Controls;

/// <summary>
/// Interaction logic for CommentsControl.xaml
/// </summary>
public partial class CommentsControl : UserControl
{
    public CommentsControl()
    {
        InitializeComponent();
    }

    private static IEnumerable<Run> GetAllCommentRuns(SegmentPairView segmentPairView)
    {
        var results = new List<Run>();

        results.AddRange(GetCommentRuns(segmentPairView.Source));

        results.AddRange(GetCommentRuns(segmentPairView.Target));

        return results;
    }

    private static IEnumerable<Run> GetCommentRuns(RichTextBox richTextBox)
    {
        foreach (var block in richTextBox.Document.Blocks)
        {
            if (block is Paragraph p)
            {
                foreach (var inline in p.Inlines)
                {
                    if (inline is Run r &&
                        (r.Background == Brushes.Yellow || r.Background == (Brush)App.Current.Resources[AdonisUI.Brushes.AccentBrush]))
                    {
                        yield return r;
                    }
                }
            }
        }
    }

    private void OnCommentGridSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is DataGrid dataGrid)
        {
            var segmentPairView = ElementLookupHelper.FindParent<SegmentPairView>(dataGrid);
            var runs = GetAllCommentRuns(segmentPairView);

            if (dataGrid.SelectedItem is CommentViewModel commentViewModel)
            {
                foreach (var run in runs)
                {
                    if (run.Tag == commentViewModel.CommentMarker)
                    {
                        run.Background = (Brush)App.Current.Resources[AdonisUI.Brushes.AccentBrush];
                    }
                    else
                    {
                        run.Background = Brushes.Yellow;
                    }
                }
            }
            else
            {
                foreach (var run in runs)
                {
                    run.Background = Brushes.Yellow;
                }
            }

            e.Handled = true;
        }
    }
}
