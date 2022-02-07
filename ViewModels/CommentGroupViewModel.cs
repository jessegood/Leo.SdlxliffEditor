using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;

namespace Leo.SdlxliffEditor.ViewModels;

public class CommentGroupViewModel : ObservableObject
{
    private readonly CommentMarker commentMarker;
    private readonly ISegmentPairViewModel parent;
    private ObservableCollection<CommentViewModel> comments;

    private int selectedIndex;

    public CommentGroupViewModel(int index, CommentMarker commentMarker, ISegmentPairViewModel segmentPairViewModel, bool isSource = false)
    {
        Index = index;
        this.commentMarker = commentMarker;
        parent = segmentPairViewModel;
        IsSource = isSource;
        comments = CreateComments();

        AddCommentCommand = new RelayCommand(OnAddComment);
        DeleteCommentCommand = new RelayCommand<DataGrid>(OnDeleteComment, CanDeletedCommandExecute);
    }

    public RelayCommand AddCommentCommand { get; }

    public CommentMarker CommentMarker => commentMarker;

    public ObservableCollection<CommentViewModel> CommentsCollection
    {
        get => comments;
        set => SetProperty(ref comments, value);
    }

    public RelayCommand<DataGrid> DeleteCommentCommand { get; }

    public int Index { get; }

    public bool IsSource { get; }

    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            SetProperty(ref selectedIndex, value);
            DeleteCommentCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanDeletedCommandExecute(DataGrid dataGrid)
    {
        return SelectedIndex >= 0;
    }

    private void CheckCommentCount()
    {
        // Remove from the comments list if the count is zero
        // and remove the comment marker completely
        if (CommentsCollection.Count == 0)
        {
            commentMarker.RemoveContentContainerFromParent();
            parent.Comments.Remove(this);
        }
    }

    private ObservableCollection<CommentViewModel> CreateComments()
    {
        ObservableCollection<CommentViewModel> ret = new();

        foreach (var comment in commentMarker.CommentDefinition.Comments)
        {
            ret.Add(new CommentViewModel(commentMarker, comment));
        }

        return ret;
    }

    private string GetVersion()
    {
        return (CommentsCollection.Count + 1).ToString("F1", CultureInfo.InvariantCulture);
    }

    private void OnAddComment()
    {
        var comment = new Comment(string.Empty)
        {
            Date = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture),
            User = string.Empty,
            Version = GetVersion(),
            Severity = CommentSeverity.Low
        };

        // Add to comment marker and observable collection
        commentMarker.CommentDefinition.Comments.Add(comment);
        CommentsCollection.Add(new CommentViewModel(commentMarker, comment));
    }

    private void OnDeleteComment(DataGrid dataGrid)
    {
        if (CanDeletedCommandExecute(dataGrid) && dataGrid.SelectedItems != null)
        {
            do
            {
                var commentViewModel = (CommentViewModel)dataGrid.SelectedItems[0];

                // Remove from comment marker
                commentMarker.CommentDefinition.Comments.Remove(commentViewModel.Comment);

                // Remove from observable collection
                CommentsCollection.Remove(commentViewModel);
            } while (dataGrid.SelectedItems.Count > 0);

            CheckCommentCount();
        }
    }
}
