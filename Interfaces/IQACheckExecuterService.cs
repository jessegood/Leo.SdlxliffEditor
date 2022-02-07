using Leo.SdlxliffEditor.QACheckers;
using System.Collections.Generic;

namespace Leo.SdlxliffEditor.Interfaces;

public interface IQACheckExecuterService : IExecuterServiceBase
{
    IQASettingsViewModel Settings { get; }

    IEnumerable<QACheckerBase> GetResults();

    void RemoveResult(int index);
}