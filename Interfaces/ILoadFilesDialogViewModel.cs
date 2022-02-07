using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Leo.SdlxliffEditor.Interfaces;

public interface ILoadFilesDialogViewModel
{
    event Action FileListCleared;

    ObservableCollection<string> FilePaths { get; set; }

    bool HasFiles { get; }

    void LoadStoredFiles(List<string> fileList);

    bool? ShowDialog();
}