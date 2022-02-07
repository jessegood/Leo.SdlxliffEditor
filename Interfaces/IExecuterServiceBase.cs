using Leo.Sdlxliff.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Leo.SdlxliffEditor.Interfaces;

public interface IExecuterServiceBase
{
    public List<ISdlxliffDocument> Documents { get; }

    public bool HasFiles { get; }

    List<string> SdlxliffFiles { get; }

    void ClearFiles();
    
    Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken);

    void LoadFiles(IEnumerable<string> sdlxliffFiles);

    Task SaveFilesAsync();
}