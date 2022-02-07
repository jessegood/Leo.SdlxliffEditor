using Leo.Sdlxliff;
using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leo.SdlxliffEditor.Services;

public class SdlxliffFileService : ISdlxliffFileService
{
    public event Action OnFilesLoaded;

    public List<string> FileList { get; } = new();

    public bool HasFiles => FileList.Count > 0;

    public void ClearFiles()
    {
        FileList.Clear();
    }

    public async IAsyncEnumerable<ISdlxliffDocument> GetSdlxliffDocumentsAsync()
    {
        foreach (var file in FileList)
        {
            yield return await SdlxliffDocument.LoadAsync(file);
        }
    }

    public void LoadFiles(IEnumerable<string> fileNames)
    {
        FileList.AddRange(fileNames);
        OnFilesLoaded?.Invoke();
    }

    public async Task SaveFilesAsync(IEnumerable<ISdlxliffDocument> documents)
    {
        foreach (var pair in documents.Zip(FileList, (d, f) => new { Document = d, FilePath = f }))
        {
            await pair.Document.SaveAsAsync(pair.FilePath);
        }
    }
}
