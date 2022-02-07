using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace Leo.SdlxliffEditor.ViewModels;

public class CommentViewModel : ObservableObject
{
    public CommentViewModel(CommentMarker commentMarker, Comment comment)
    {
        CommentMarker = commentMarker;
        Comment = comment;
    }

    public Comment Comment { get; }

    public CommentMarker CommentMarker { get; }

    public CommentSeverity CommentSeverity
    {
        get => Comment.Severity;
        set => SetProperty(Comment.Severity, value, Comment, (comment, severity) => comment.Severity = severity);
    }
    public string Contents
    {
        get => Comment.Contents;
        set => SetProperty(Comment.Contents, value, Comment, (comment, contents) => comment.Contents = contents);
    }

    public DateTime Date => DateTime.Parse(Comment.Date);

    public string User
    {
        get => Comment.User;
        set => SetProperty(Comment.User, value, Comment, (comment, user) => comment.User = user);
    }

    public string Version => Comment.Version;
}
