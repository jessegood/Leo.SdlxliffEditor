using Leo.Sdlxliff.Interfaces;
using System.Collections.Generic;

namespace Leo.SdlxliffEditor.QACheckers;

public abstract class QACheckerBase
{
    public QACheckerBase(string name, bool isCaseSensitive, bool storeResults = true)
    {
        Name = (string)App.Current.Resources[name];
        CaseSensitive = isCaseSensitive;
        Results = storeResults ? new Dictionary<string, List<int>>() : null;
    }

    public string Name { get; }

    public Dictionary<string, List<int>> Results { get; }

    protected bool CaseSensitive { get; }

    public abstract void Check(ISegmentPair segmentPair, string fileName, int index);

    protected void AddToResults(string fileName, int index)
    {
        if (Results.ContainsKey(fileName))
        {
            Results[fileName].Add(index);
        }
        else
        {
            Results.Add(fileName, new List<int> { index });
        }
    }
}