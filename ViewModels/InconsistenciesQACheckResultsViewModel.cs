using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.QACheckers;
using System.Collections.Generic;
using System.Linq;

namespace Leo.SdlxliffEditor.ViewModels;

public class InconsistenciesQACheckResultsViewModel : QACheckResultsViewModelBase
{
    private readonly Dictionary<string, Dictionary<string, List<(string, int)>>> inconsistencies;
    private readonly bool isTargetInconsistencies;
    private readonly InconsistenciesQAChecker qaCheckResult;

    public InconsistenciesQACheckResultsViewModel(InconsistenciesQAChecker qaCheckResult, bool isTargetInconsistencies = true)
        : base(qaCheckResult.Name)
    {
        inconsistencies = GetInconsistencies(qaCheckResult.Inconsistencies);
        ErrorCount = inconsistencies.Count;
        this.qaCheckResult = qaCheckResult;
        this.isTargetInconsistencies = isTargetInconsistencies;
    }

    public bool IsTargetInconsistencies => isTargetInconsistencies;

    public override void Add(string fileName, int rowNumber, ISegmentPair segmentPair, ISdlxliffDocument document)
    {
        var segment = (ISegment)(isTargetInconsistencies ? segmentPair.SourceSegment.DeepCopy() : segmentPair.TargetSegment.DeepCopy());

        var key = qaCheckResult.PrepareSegment(segment.ToStringWithoutCommentsAndRevisions());

        if (inconsistencies.ContainsKey(key))
        {
            if (SegmentPairs.ContainsKey(key))
            {
                SegmentPairs[key].Add(new QASegmentPairViewModel(segmentPair, ContextMenuItems, document, rowNumber, fileName));
            }
            else
            {
                SegmentPairs.Add(key, new List<IQASegmentPairViewModel> { new QASegmentPairViewModel(segmentPair, ContextMenuItems, document, rowNumber, fileName) });
            }
        }
    }

    private Dictionary<string, Dictionary<string, List<(string, int)>>> GetInconsistencies(Dictionary<string, Dictionary<string, List<(string, int)>>> inconsistencies)
    {
        // If the key points to multiple dictionaries, there is an inconsistency
        // We create a new dictionary that includes only the inconsistencies && p.Value.Keys.All(k => p.Key != k)
        Dictionary<string, Dictionary<string, List<(string, int)>>> filteredInconsistencies = new(
                inconsistencies.Where(p => p.Value.Count > 1)
            );

        return filteredInconsistencies;
    }
}