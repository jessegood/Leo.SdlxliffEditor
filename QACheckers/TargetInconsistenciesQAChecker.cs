using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Interfaces;

namespace Leo.SdlxliffEditor.QACheckers;

public sealed class TargetInconsistenciesQAChecker : InconsistenciesQAChecker
{
    public TargetInconsistenciesQAChecker(string name, bool isCaseSensitive, bool ignoreNumber, bool ignorePunctuation)
        : base(name, isCaseSensitive, ignoreNumber, ignorePunctuation)
    {
    }

    public override void Check(ISegmentPair segmentPair, string fileName, int index)
    {
        var source = segmentPair.SourceSegment.ToStringWithoutCommentsAndRevisions();
        var target = segmentPair.TargetSegment.ToStringWithoutCommentsAndRevisions();

        source = PrepareSegment(source);

        if (!string.IsNullOrWhiteSpace(source))
        {
            target = PrepareSegment(target);

            if (!string.IsNullOrWhiteSpace(target))
            {
                Add(source, target, fileName, index);
            }
        }
    }
}