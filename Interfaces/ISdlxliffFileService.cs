using Leo.Sdlxliff.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leo.SdlxliffEditor.Interfaces;

public interface ISdlxliffFileService
{
    event Action OnFilesLoaded;

    List<string> FileList { get; }
    bool HasFiles { get; }

    void ClearFiles();

    IAsyncEnumerable<ISdlxliffDocument> GetSdlxliffDocumentsAsync();

    void LoadFiles(IEnumerable<string> fileNames);

    Task SaveFilesAsync(IEnumerable<ISdlxliffDocument> documents);
}
