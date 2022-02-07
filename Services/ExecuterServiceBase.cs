using Leo.Sdlxliff;
using Leo.Sdlxliff.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Leo.SdlxliffEditor.Services;

public abstract class ExecuterServiceBase
{
    public List<ISdlxliffDocument> Documents { get; } = new();

    public bool HasFiles => SdlxliffFiles.Count > 0;

    public List<string> SdlxliffFiles { get; } = new();
    
    public void ClearFiles()
    {
        SdlxliffFiles.Clear();
    }

    public abstract Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken);

    public void LoadFiles(IEnumerable<string> sdlxliffFiles)
    {
        SdlxliffFiles.AddRange(sdlxliffFiles);
    }

    public async Task SaveFilesAsync()
    {
        foreach (var pair in Documents.Zip(SdlxliffFiles, (d, f) => new { Document = d, FilePath = f }))
        {
            await pair.Document.SaveAsAsync(pair.FilePath);
        }
    }

    protected async IAsyncEnumerable<List<ISegmentPair>> LoadSegmentPairsAsync()
    {
        await foreach (var document in LoadSdlxliffDocumentsAsync())
        {
            Documents.Add(document);
            List<ISegmentPair> segmentPairs = new();

            foreach (var tu in document.TranslationUnits.Where(t => t.HasSegmentPairs))
            {
                segmentPairs.AddRange(tu.GetSegmentPairs());
            }

            yield return segmentPairs;
        }
    }

    private async IAsyncEnumerable<ISdlxliffDocument> LoadSdlxliffDocumentsAsync()
    {
        foreach (var file in SdlxliffFiles)
        {
            yield return await SdlxliffDocument.LoadAsync(file);
        }
    }
}