using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.QACheckers;
using System.Collections.Generic;
using System.Linq;

namespace Leo.SdlxliffEditor.ViewModels;

public class QACheckResultsViewModel : QACheckResultsViewModelBase
{
    private readonly Dictionary<string, List<int>> results;

    public QACheckResultsViewModel(QACheckerBase qaCheckResult)
        : base(qaCheckResult.Name, qaCheckResult.Results.Select(p => p.Value.Count).Sum())
    {
        results = qaCheckResult.Results;
    }

    public override void Add(string fileName, int rowNumber, ISegmentPair segmentPair, ISdlxliffDocument document)
    {
        if (results.TryGetValue(fileName, out var rowNumbers))
        {
            if (rowNumbers.Contains(rowNumber))
            {
                if (SegmentPairs.ContainsKey(fileName))
                {
                    SegmentPairs[fileName].Add(new QASegmentPairViewModel(segmentPair, ContextMenuItems, document, rowNumber));
                }
                else
                {
                    SegmentPairs.Add(fileName, new List<IQASegmentPairViewModel> { new QASegmentPairViewModel(segmentPair, ContextMenuItems, document, rowNumber) });
                }
            }
        }
    }
}