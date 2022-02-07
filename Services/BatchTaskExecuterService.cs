using Leo.SdlxliffEditor.BatchTasks;
using Leo.SdlxliffEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Leo.SdlxliffEditor.Services;

public class BatchTaskExecuterService : ExecuterServiceBase, IBatchTaskExecuterService
{
    private List<BatchTaskBase> batchTasks;

    public bool IsTaskChecked => batchTasks.Any(t => t.IsChecked);

    public void AddBatchTasks(IEnumerable<BatchTaskBase> batchTasks)
    {
        this.batchTasks = new List<BatchTaskBase>(batchTasks);
    }

    public override async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        var currentProgress = 0;
        var progressInterval = 100 / SdlxliffFiles.Count;

        await foreach (var segmentPairs in LoadSegmentPairsAsync())
        {
            foreach (var segmentPair in segmentPairs)
            {
                foreach (var batchTask in batchTasks)
                {
                    if (batchTask.IsChecked)
                    {
                        batchTask.ProcessSegmentPair(segmentPair);
                    }
                }
            }

            currentProgress += progressInterval;
            progress.Report(currentProgress);
        }

        progress.Report(100);
    }
}
