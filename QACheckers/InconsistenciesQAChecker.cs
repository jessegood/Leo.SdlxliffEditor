using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Leo.SdlxliffEditor.QACheckers;

public abstract class InconsistenciesQAChecker : QACheckerBase
{
    private readonly bool ignoreNumber;
    private readonly bool ignorePunctuation;

    public InconsistenciesQAChecker(string name, bool isCaseSensitive, bool ignoreNumber, bool ignorePunctuation)
        : base(name, isCaseSensitive, false)
    {
        this.ignoreNumber = ignoreNumber;
        this.ignorePunctuation = ignorePunctuation;
    }

    // Data structure:
    // A single source linked to multiple targets.
    // Each target can be associated with multiple filename/row number pairs
    // If a single source has only one target, then it is not an inconsistency
    //
    //                 --> target --> filename/row number (multiple)
    //
    // source (single) --> target
    //                 --> target
    public Dictionary<string, Dictionary<string, List<(string, int)>>> Inconsistencies { get; } = new Dictionary<string, Dictionary<string, List<(string, int)>>>();

    public string PrepareSegment(string segment)
    {
        // Use the culture rules of the current culture
        if (!CaseSensitive)
        {
            segment = segment.ToUpper(CultureInfo.CurrentCulture);
        }

        if (ignoreNumber)
        {
            segment = Regex.Replace(segment, @"\d+", "");
        }

        if (ignorePunctuation)
        {
            segment = Regex.Replace(segment, @"\p{P}+", "");
        }

        // Bi-directional text
        // right-to-left mark \u200F
        // left-to-right mark \u200E
        // Arabic letter mark \u061C
        segment = Regex.Replace(segment, "\u200F|\u200E|\u061C", "");

        // Trim any whitespace
        return segment.Trim();
    }

    protected void Add(string key1, string key2, string fileName, int index)
    {
        if (Inconsistencies.ContainsKey(key1))
        {
            if (Inconsistencies[key1].ContainsKey(key2))
            {
                Inconsistencies[key1][key2].Add((fileName, index));
            }
            else
            {
                Inconsistencies[key1].Add(key2, new List<(string, int)> { (fileName, index) });
            }
        }
        else
        {
            Inconsistencies.Add(key1, new Dictionary<string, List<(string, int)>> { { key2, new List<(string, int)> { (fileName, index) } } });
        }
    }
}