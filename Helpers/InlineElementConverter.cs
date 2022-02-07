using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Leo.SdlxliffEditor.Helpers;

public static class InlineElementConverter
{
    // Brushes
    private static readonly Brush lockedContentBackground = (Brush)App.Current.FindResource(AdonisUI.Brushes.Layer2BorderBrush);

    private static readonly Brush tagBackground = (Brush)App.Current.FindResource(AdonisUI.Brushes.Layer3BackgroundBrush);

    public static IEnumerable<Inline> Convert(IEnumerable<ITranslationUnitContent> content, bool useFullTagText)
    {
        List<Inline> inlines = new();

        foreach (var unit in content)
        {
            switch (unit.ContentType)
            {
                case TranslationUnitContentType.CommentsMarker:
                    var commentMarker = (CommentMarker)unit;
                    inlines.AddRange(CreateComment(commentMarker, useFullTagText));
                    break;

                case TranslationUnitContentType.LocationMarker:
                    // TODO: Not sure if I need to handle this one
                    break;

                case TranslationUnitContentType.CustomMarker:
                    // TODO: Need to find some examples
                    break;

                case TranslationUnitContentType.LockedContent:
                    var lockedContent = (LockedContent)unit;
                    inlines.Add(CreateLockedContent(lockedContent, useFullTagText));
                    break;

                case TranslationUnitContentType.Placeholder:
                    var placeholder = (Placeholder)unit;
                    inlines.Add(CreateInlineTag(useFullTagText ? placeholder.Contents : placeholder.Name, tagBackground, unit, useFullTagText));
                    break;

                case TranslationUnitContentType.RevisionMarker:
                    var revision = (RevisionMarker)unit;
                    inlines.AddRange(CreateRevision(revision, useFullTagText));
                    break;

                case TranslationUnitContentType.PairedTag:
                    var tagPair = (TagPair)unit;

                    inlines.Add(CreateInlineTag(useFullTagText ? tagPair.BeginPairedTag.Contents : tagPair.BeginPairedTag.Name, tagBackground, unit, useFullTagText));
                    inlines.AddRange(Convert(tagPair.Contents, useFullTagText));
                    inlines.Add(CreateInlineTag(tagPair.EndPairedTag.Name, tagBackground, null, useFullTagText));

                    break;

                case TranslationUnitContentType.StructureTag:
                    var structureTag = (StructureTag)unit;
                    inlines.Add(CreateInlineTag(structureTag.Name, tagBackground, unit, useFullTagText));
                    break;

                case TranslationUnitContentType.Text:
                    var text = (IText)unit;
                    inlines.Add(new Run(text.Contents));
                    break;

                default:
                    throw new ArgumentException($"Unknown content type {unit.ContentType}");
            }
        }

        return inlines;
    }

    private static IEnumerable<Inline> CreateComment(CommentMarker commentMarker, bool useFullTagText)
    {
        if (commentMarker.Contents.Count == 1 && commentMarker.Contents[0] is IText text)
        {
            return new[] { new Run()
                {
                    Text = text.Contents,
                    Background = Brushes.Yellow,
                    Tag = TagTypeConverter.ConvertToTagType(commentMarker)
                }};
        }
        else
        {
            var commentContent = Convert(commentMarker.Contents, useFullTagText);
            foreach (var inline in commentContent)
            {
                if (inline is Run r)
                {
                    r.Background = Brushes.Yellow;
                    r.Tag = TagTypeConverter.ConvertToTagType(commentMarker);
                }
            }

            return commentContent;
        }
    }

    private static InlineUIContainer CreateInlineTag(string contents, Brush background, ITranslationUnitContent content, bool useFullTagText)
    {
        return new InlineUIContainer(new Border()
        {
            Child = new TextBlock(new Run(content is not null ? (useFullTagText ? contents : $"<{ contents }>") : $"</{ contents }>")),
            Background = background,
            // CornerRadius = new CornerRadius(5),
        })
        {
            Tag = content switch
            {
                Placeholder placeholder => TagTypeConverter.ConvertToTagType(placeholder),
                StructureTag structureTag => TagTypeConverter.ConvertToTagType(structureTag),
                TagPair tagPair => TagTypeConverter.ConvertToTagType(tagPair),
                _ => null
            },
            BaselineAlignment = BaselineAlignment.Bottom
        };
    }

    private static Inline CreateLockedContent(LockedContent lockedContent, bool useFullTagText)
    {
        var transUnit = lockedContent.LockedTranslationUnit;
        if (transUnit is not null)
        {
            var textBlock = new TextBlock();
            textBlock.Inlines.AddRange(Convert(transUnit.Source.Contents, useFullTagText));

            return new InlineUIContainer(new Border()
            {
                Child = textBlock,
                Background = lockedContentBackground
            })
            {
                Tag = TagTypeConverter.ConvertToTagType(lockedContent),
                BaselineAlignment = BaselineAlignment.Bottom
            };
        }

        throw new ArgumentNullException("The locked translation unit was null", nameof(transUnit));
    }

    private static IEnumerable<Inline> CreateRevision(RevisionMarker revision, bool useFullTagText)
    {
        if (revision.Contents.Count == 1 && revision.Contents[0] is IText text)
        {
            return new[] { new Run()
                {
                    Text = text.Contents,
                    TextDecorations = GetTextDecoration(revision.RevisionDefinition.Type),
                    Foreground = Brushes.Red,
                    Tag = TagTypeConverter.ConvertToTagType(revision)
                }};
        }
        else
        {
            var revisionContent = Convert(revision.Contents, useFullTagText);
            foreach (var inline in revisionContent)
            {
                if (inline is Run r)
                {
                    r.TextDecorations = GetTextDecoration(revision.RevisionDefinition.Type);
                    r.Foreground = Brushes.Red;
                    r.Tag = TagTypeConverter.ConvertToTagType(revision);
                }
            }

            return revisionContent;
        }
    }

    private static TextDecorationCollection GetTextDecoration(RevisionType type)
    {
        return type switch
        {
            RevisionType.Insert => TextDecorations.Underline,
            RevisionType.Delete => TextDecorations.Strikethrough,
            RevisionType.Unchanged => new TextDecorationCollection(),
            // TODO: Handle TQA revisions
            RevisionType.FeedbackAdded => TextDecorations.Underline,
            RevisionType.FeedbackDeleted => TextDecorations.Strikethrough,
            RevisionType.FeedbackComment => TextDecorations.OverLine,
            _ => throw new ArgumentException($"Unexpected revision type {type}"),
        };
    }
}