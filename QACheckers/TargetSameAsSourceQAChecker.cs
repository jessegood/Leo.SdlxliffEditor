using Leo.Sdlxliff.Interfaces;
using System;

namespace Leo.SdlxliffEditor.QACheckers;

public sealed class TargetSameAsSourceQAChecker : QACheckerBase
{
    public TargetSameAsSourceQAChecker(string name, bool isCaseSensitive) : base(name, isCaseSensitive)
    {
    }

    public override void Check(ISegmentPair segmentPair, string fileName, int index)
    {
        var source = segmentPair.SourceSegment.ToString();
        var target = segmentPair.TargetSegment.ToString();

        if (source.Equals(target, CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase))
        {
            AddToResults(fileName, index);
        }
    }
}
