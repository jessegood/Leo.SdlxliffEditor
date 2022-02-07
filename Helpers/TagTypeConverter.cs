using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using Leo.SdlxliffEditor.TagTypes;
using System;
using System.Linq;

namespace Leo.SdlxliffEditor.Helpers;

internal static class TagTypeConverter
{
    internal static ITranslationUnitContent ConvertToContent(TagPairTagType tagPairType, ISdlxliffDocument sdlxliffDocument)
    {
        var tagPair = new TagPair()
        {
            Id = tagPairType.Id,
            Start = tagPairType.Start,
            End = tagPairType.End,
            StartRevisionId = tagPairType.StartRevisionId,
            EndRevisionId = tagPairType.EndRevisionId,
            FormatId = tagPairType.FormatId
        };

        PropertySetter.SetTagDefinitionsForTagPair(tagPair, sdlxliffDocument.Header.TagDefinitions, sdlxliffDocument.RevisionDefinitions);

        return tagPair;
    }

    internal static ITranslationUnitContent ConvertToContent(PlaceholderTagType placeholderTagType, ISdlxliffDocument sdlxliffDocument)
    {
        if (sdlxliffDocument.Header.TagDefinitions.PlaceholderInfo.TryGetValue(placeholderTagType.Id, out var placeholder))
        {
            return PlaceholderTagCreator.CreatePlaceholder(placeholderTagType.Id, placeholder);
        }
        else
        {
            throw new ArgumentException($"Could not find placeholder id: { placeholderTagType.Id }");
        }
    }

    internal static ITranslationUnitContent ConvertToContent(RevisionMarkerTagType revisionMarkerTagType, ISdlxliffDocument sdlxliffDocument)
    {
        var revisionMarker = sdlxliffDocument.RevisionMarkers.FirstOrDefault(r => r.Id == revisionMarkerTagType.Id);
        return revisionMarker ?? throw new ArgumentException($"Could not find revision marker id: { revisionMarkerTagType.Id }", nameof(revisionMarkerTagType));
    }

    internal static ITranslationUnitContent ConvertToContent(CommentMarkerTagType commentMarkerTagType, ISdlxliffDocument sdlxliffDocument)
    {
        var commentMarker = sdlxliffDocument.CommentMarkers.FirstOrDefault(c => c.Id == commentMarkerTagType.Id);
        return commentMarker ?? throw new ArgumentException($"Could not find comment marker id: { commentMarkerTagType.Id }", nameof(commentMarkerTagType));
    }

    internal static ITranslationUnitContent ConvertToContent(StructureTagType structureTagType, ISdlxliffDocument sdlxliffDocument)
    {
        if (sdlxliffDocument.Header.TagDefinitions.StructureTagInfo.TryGetValue(structureTagType.Id, out var structureTag))
        {
            return PlaceholderTagCreator.CreateStructureTag((structureTagType.Id, structureTagType.XId), structureTag);
        }
        else
        {
            throw new ArgumentException($"Could not find structure tag id: { structureTagType.Id }", nameof(structureTagType));
        }
    }

    internal static ITranslationUnitContent ConvertToContent(LockContentTagType lockContentTagType, ISdlxliffDocument sdlxliffDocument)
    {
        var lockedTU = sdlxliffDocument.TranslationUnitsAll.FirstOrDefault(tu => tu.TranslationUnitId == lockContentTagType.XId);
        if (lockedTU is not null)
        {
            return PlaceholderTagCreator.CreateLockedContent((lockContentTagType.Id, lockContentTagType.XId), lockedTU);
        }
        else
        {
            throw new ArgumentException($"Could not find locked translation unit xid: {lockContentTagType.XId}");
        }
    }

    internal static TagTypeBase ConvertToTagType(CommentMarker content)
    {
        return new CommentMarkerTagType()
        {
            Id = content.Id
        };
    }

    internal static TagTypeBase ConvertToTagType(LockedContent content)
    {
        return new LockContentTagType()
        {
            Id = content.Id,
            XId = content.XId
        };
    }

    internal static TagTypeBase ConvertToTagType(StructureTag content)
    {
        return new LockContentTagType()
        {
            Id = content.Id,
            XId = content.XId
        };
    }

    internal static TagTypeBase ConvertToTagType(TagPair content)
    {
        return new TagPairTagType()
        {
            Id = content.Id,
            Start = content.Start,
            End = content.End,
            FormatId = content.FormatId,
            StartRevisionId = content.StartRevisionId,
            EndRevisionId = content.EndRevisionId,
            BeginPairedTagName = content.BeginPairedTag.Name,
            BeginPairedTagContents = content.BeginPairedTag.Contents
        };
    }

    internal static TagTypeBase ConvertToTagType(Placeholder placeholder)
    {
        return new PlaceholderTagType()
        {
            Id = placeholder.Id,
            Name = placeholder.Name,
            Contents = placeholder.Contents
        };
    }

    internal static TagTypeBase ConvertToTagType(RevisionMarker revision)
    {
        return new RevisionMarkerTagType()
        {
            MarkerType = revision.MarkerType,
            Id = revision.Id
        };
    }
}