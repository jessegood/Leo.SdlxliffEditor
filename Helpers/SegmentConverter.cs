using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using Leo.SdlxliffEditor.Exceptions;
using Leo.SdlxliffEditor.TagTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Leo.SdlxliffEditor.Helpers;

public static class SegmentConverter
{
    public static void Update(FlowDocument document, ISegment segment, ISdlxliffDocument sdlxliffDocument)
    {
        List<ITranslationUnitContent> contents = new();
        Stack<TagPair> tagPairHolder = new();

        foreach (var block in document.Blocks)
        {
            if (block is Paragraph p)
            {
                var inlines = p.Inlines.ToList();

                for (int i = 0; i < inlines.Count; i++)
                {
                    switch (inlines[i])
                    {
                        case InlineUIContainer inlineUIContainer:
                            HandleTag(inlineUIContainer, contents, tagPairHolder, sdlxliffDocument);
                            break;

                        case Run run:
                            var content = HandleRun(run, segment, inlines, i, sdlxliffDocument);
                            // Add to the count how many items you need to skip
                            // after reading comments and revisions
                            i += content.Item2;
                            AddContent(contents, tagPairHolder, content.Item1);
                            break;

                        case LineBreak:
                        case Span:
                        default:
                            throw new InvalidOperationException($"Unexpected type {inlines[i].GetType()}");
                    }
                }
            }
        }

        CheckForMissingTags(tagPairHolder);

        UpdateContents(segment, contents);
    }

    private static void AddContent(IList<ITranslationUnitContent> contents, Stack<TagPair> tagPairHolder, ITranslationUnitContent content)
    {
        // If the tagPairholder contains a tag pair, we add the content to that
        if (tagPairHolder.Count > 0)
        {
            // Don't forget to set the parent!
            content.Parent = tagPairHolder.Peek();

            AddUpdatedContent(tagPairHolder.Peek().Contents, content);
        }
        else
        {
            AddUpdatedContent(contents, content);
        }
    }

    private static void AddUpdatedContent(IList<ITranslationUnitContent> contents, ITranslationUnitContent content)
    {
        // If the list is empty, we just add the content and return
        if (contents.Count == 0)
        {
            contents.Add(content);
            return;
        }

        // Double check that adjacent nodes are not both text
        // If they are we merge the text together
        if (contents.Last() is IText text1 && content is IText text2)
        {
            text1.Contents += text2.Contents;
        }
        else
        {
            contents.Add(content);
        }
    }

    private static void CheckForMissingTags(Stack<TagPair> tagPairHolder)
    {
        if (tagPairHolder.Count > 0)
        {
            List<MissingTagException> exceptions = new();
            while (tagPairHolder.Count > 0)
            {
                var tagPair = tagPairHolder.Pop();
                exceptions.Add(new MissingTagException($"End tag for <{ tagPair.BeginPairedTag.Name }> could not be found. Please fix to update the segment."));
            }

            throw new AggregateException(exceptions);
        }
    }
    private static (ITranslationUnitContent, int) HandleRun(Run run, ISegment segment, IList<Inline> inlines, int i, ISdlxliffDocument sdlxliffDocument)
    {
        switch (run.Tag)
        {
            case CommentMarkerTagType commentMarkerTagType:
                var commentMarker = (CommentMarker)TagTypeConverter.ConvertToContent(commentMarkerTagType, sdlxliffDocument);
                commentMarker.Contents = new List<ITranslationUnitContent>
                                        {
                                            new Text(run.Text)
                                            {
                                                Parent = commentMarker
                                            }
                                        };

                var commentCheck = HasMultipleContent(commentMarker, inlines, i);
                if (commentCheck.Item1)
                {
                    HandleRunContent(commentMarker, inlines, commentCheck.Item2, i, sdlxliffDocument);
                }

                return (commentMarker, commentCheck.Item2);

            case RevisionMarkerTagType revisionMarkerTagType:
                var revisionMarker = (RevisionMarker)TagTypeConverter.ConvertToContent(revisionMarkerTagType, sdlxliffDocument);
                revisionMarker.Contents = new List<ITranslationUnitContent>
                                            {
                                                new Text(run.Text)
                                                {
                                                    Parent = revisionMarker
                                                }
                                            };

                var revisionCheck = HasMultipleContent(revisionMarker, inlines, i);
                if (revisionCheck.Item1)
                {
                    HandleRunContent(revisionMarker, inlines, revisionCheck.Item2, i, sdlxliffDocument);
                }

                return (revisionMarker, revisionCheck.Item2);

            default:
                return (new Text(run.Text) { Parent = segment as TranslationUnitContentContainer }, 0);
        }
    }

    private static void HandleRunContent(TranslationUnitContentContainer container, IList<Inline> inlines, int j, int i, ISdlxliffDocument sdlxliffDocument)
    {
        Stack<TagPair> tagPairHolder = new();

        for (int k = i + 1; k <= j; k++)
        {
            switch (inlines[k])
            {
                case InlineUIContainer inlineUIContainer:
                    HandleTag(inlineUIContainer, container.Contents, tagPairHolder, sdlxliffDocument);
                    break;

                case Run run:
                    var content = new Text(run.Text) { Parent = container };
                    AddContent(container.Contents, tagPairHolder, content);
                    break;

                case LineBreak:
                case Span:
                default:
                    throw new InvalidOperationException($"Unexpected type {inlines[k].GetType()}");
            }
        }

        CheckForMissingTags(tagPairHolder);
    }

    private static void HandleTag(InlineUIContainer inlineUIContainer, IList<ITranslationUnitContent> contents, Stack<TagPair> tagPairHolder, ISdlxliffDocument sdlxliffDocument)
    {
        // End pair tags are null and are ignored because we store the tag pairs
        if (inlineUIContainer.Tag is TagTypeBase tagType)
        {
            // If we hit a tag pair, we add
            // it to the stack and move on to the next inline
            if (tagType is TagPairTagType tagPairType)
            {
                var tp = (TagPair)TagTypeConverter.ConvertToContent(tagPairType, sdlxliffDocument);
                tagPairHolder.Push(tp);
                return;
            }

            AddContent(contents, tagPairHolder, tagType switch
            {
                LockContentTagType lockContentTagType => (LockedContent)TagTypeConverter.ConvertToContent(lockContentTagType, sdlxliffDocument),
                PlaceholderTagType placeholderTagType => (Placeholder)TagTypeConverter.ConvertToContent(placeholderTagType, sdlxliffDocument),
                StructureTagType structureTagType => (StructureTag)TagTypeConverter.ConvertToContent(structureTagType, sdlxliffDocument),
                _ => throw new ArgumentException($"Unknown tag type { tagType.GetType() }")
            });
        }
        else
        {
            if (tagPairHolder.Count > 0)
            {
                // Pop the tag pair off the stack as we hit the end pair tag
                // and add the contents
                var tagPair = tagPairHolder.Pop();
                AddContent(contents, tagPairHolder, tagPair);
            }
            else
            {
                // We hit and end tag but the start tag is missing
                var run = ((TextBlock)((Border)inlineUIContainer.Child).Child).Inlines.FirstInline as Run;

                throw new MissingTagException($"The start tag for { run.Text } is missing. Please fix to update the segment.");
            }
        }
    }

    private static (bool, int) HasMultipleContent(ITranslationUnitContent content, IList<Inline> inlines, int i)
    {
        (bool, int) ret = (false, 0);

        for (int j = i + 1; j < inlines.Count; j++)
        {
            if (inlines[j].Tag != null && IsSameContent(inlines[j].Tag, content))
            {
                ret = (true, j);
            }
        }

        return ret;
    }

    private static bool IsSameContent(object tag, ITranslationUnitContent content)
        => (tag, content) switch
        {
            { tag: CommentMarkerTagType commentMarkerTagType, content: CommentMarker commentMarker }
                => commentMarkerTagType.Id == commentMarker.Id,
            { tag: LockContentTagType lockContentTagType, content: LockedContent lockedContent }
                => lockContentTagType.Id == lockedContent.Id && lockContentTagType.XId == lockedContent.XId,
            { tag: PlaceholderTagType placeholderTagType, content: Placeholder placeholder }
                => placeholderTagType.Id == placeholder.Id,
            { tag: RevisionMarkerTagType revisionMarkerTagType, content: RevisionMarker revisionMarker }
                => revisionMarkerTagType.Id == revisionMarker.Id && revisionMarkerTagType.MarkerType == revisionMarker.MarkerType,
            { tag: StructureTagType structureTagType, content: StructureTag structureTag }
                => structureTagType.Id == structureTag.Id && structureTagType.XId == structureTag.XId,
            { tag: TagPairTagType tagPairTagType, content: TagPair tagPair }
                => tagPairTagType.Id == tagPair.Id && tagPairTagType.FormatId == tagPair.FormatId,
            _ => false
        };

    private static void UpdateContents(ISegment segment, List<ITranslationUnitContent> contents)
    {
        segment.Contents.Clear();
        // Set parent to segment
        foreach (var c in contents)
        {
            c.Parent = (TranslationUnitContentContainer)segment;
        }

        ((List<ITranslationUnitContent>)segment.Contents).AddRange(contents);
    }
}