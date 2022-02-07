using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;

namespace Leo.SdlxliffEditor.QACheckers;

public sealed class UnconfirmedSegmentsQAChecker : QACheckerBase
{
    public UnconfirmedSegmentsQAChecker(string name, bool isCaseSensitive)
        : base(name, isCaseSensitive)
    {
    }

    public override void Check(ISegmentPair segmentPair, string fileName, int index)
    {
        // Remember not to use HasFlag when checking for the value 0
        if (segmentPair.ConfirmationLevel.Equals(ConfirmationLevel.Unspecified) ||
            segmentPair.ConfirmationLevel.HasFlag(ConfirmationLevel.Draft) ||
            segmentPair.ConfirmationLevel.HasFlag(ConfirmationLevel.NotTranslated))
        {
            AddToResults(fileName, index);
        }
    }
}
