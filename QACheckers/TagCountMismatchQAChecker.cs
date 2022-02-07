using Leo.Sdlxliff.Interfaces;
using System.Linq;

namespace Leo.SdlxliffEditor.QACheckers;

public class TagCountMismatchQAChecker : TagQAChecker
{
    public TagCountMismatchQAChecker(string name, bool isCaseSensitive) : base(name, isCaseSensitive)
    {
    }

    public override void Check(ISegmentPair segmentPair, string fileName, int index)
    {
        var sourceTags = GetTags(segmentPair.SourceSegment);
        var targetTags = GetTags(segmentPair.TargetSegment);

        if (sourceTags.Count() != targetTags.Count())
        {
            AddToResults(fileName, index);
        }
    }
}