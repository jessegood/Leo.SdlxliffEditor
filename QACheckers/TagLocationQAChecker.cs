using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using System.Linq;

namespace Leo.SdlxliffEditor.QACheckers;

public class TagLocationQAChecker : TagQAChecker
{
    public TagLocationQAChecker(string name, bool isCaseSensitive) : base(name, isCaseSensitive)
    {
    }

    public override void Check(ISegmentPair segmentPair, string fileName, int index)
    {
        var sourceFirstContent = segmentPair.SourceSegment.Contents.First();
        if (IsTag(sourceFirstContent))
        {
            var targetFirstContent = segmentPair.TargetSegment.Contents.First();
            if (sourceFirstContent.ContentType != targetFirstContent.ContentType)
            {
                AddToResults(fileName, index);
            }
        }

        var sourceLastContent = segmentPair.SourceSegment.Contents.Last();
        if (IsTag(sourceLastContent))
        {
            var targetLastContent = segmentPair.TargetSegment.Contents.Last();
            if (sourceLastContent.ContentType != targetLastContent.ContentType)
            {
                AddToResults(fileName, index);
            }
        }
    }

    private bool IsTag(ITranslationUnitContent content)
    {
        switch (content.ContentType)
        {
            case TranslationUnitContentType.LockedContent:
            case TranslationUnitContentType.Placeholder:
            case TranslationUnitContentType.PairedTag:
            case TranslationUnitContentType.StructureTag:
                return true;

            default:
                break;
        }

        return false;
    }
}