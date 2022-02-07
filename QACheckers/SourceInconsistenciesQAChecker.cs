using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Interfaces;

namespace Leo.SdlxliffEditor.QACheckers;

public sealed class SourceInconsistenciesQAChecker : InconsistenciesQAChecker
{
    public SourceInconsistenciesQAChecker(string name, bool isCaseSensitive, bool ignoreNumber, bool ignorePunctuation)
        : base(name, isCaseSensitive, ignoreNumber, ignorePunctuation)
    {
    }

    public override void Check(ISegmentPair segmentPair, string fileName, int index)
    {
        var source = segmentPair.SourceSegment.ToStringWithoutCommentsAndRevisions();
        var target = segmentPair.TargetSegment.ToStringWithoutCommentsAndRevisions();

        target = PrepareSegment(target);

        if (!string.IsNullOrWhiteSpace(target))
        {
            source = PrepareSegment(source);

            if (!string.IsNullOrWhiteSpace(source))
            {
                Add(target, source, fileName, index);
            }
        }
    }
}