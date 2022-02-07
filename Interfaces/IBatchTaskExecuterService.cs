using Leo.SdlxliffEditor.BatchTasks;
using System.Collections.Generic;

namespace Leo.SdlxliffEditor.Interfaces;

public interface IBatchTaskExecuterService : IExecuterServiceBase
{
    bool IsTaskChecked { get; }

    void AddBatchTasks(IEnumerable<BatchTaskBase> batchTasks);
}
