using Leo.Sdlxliff.Interfaces;

namespace Leo.SdlxliffEditor.QACheckers;

public sealed class EmptyTargetSegmentsQAChecker : QACheckerBase
{
    public EmptyTargetSegmentsQAChecker(string name, bool isCaseSensitive)
        : base(name, isCaseSensitive)
    {
    }

    public override void Check(ISegmentPair segmentPair, string fileName, int index)
    {
        if (segmentPair.TargetSegment.Contents.Count == 0)
        {
            AddToResults(fileName, index);
        }
    }
}
