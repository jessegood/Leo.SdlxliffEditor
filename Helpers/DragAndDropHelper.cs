using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Leo.SdlxliffEditor.Helpers;

public static class DragAndDropHelper
{
    public static ObservableCollection<string> GetSdlxliffs(object data)
    {
        var sdlxliffFiles = new ObservableCollection<string>();
        if (data is DataObject d)
        {
            if (d.GetDataPresent(DataFormats.FileDrop))
            {
                var paths = (string[])d.GetData(DataFormats.FileDrop, false);

                foreach (var path in paths)
                {
                    if (Directory.Exists(path))
                    {
                        foreach (var sdlxliff in Directory.EnumerateFiles(path, "*.sdlxliff", SearchOption.AllDirectories))
                        {
                            if (IsSdlxliff(sdlxliff))
                            {
                                sdlxliffFiles.Add(sdlxliff);
                            }
                        }
                    }
                    else if (File.Exists(path) && IsSdlxliff(path))
                    {
                        sdlxliffFiles.Add(path);
                    }
                }
            }
        }

        return sdlxliffFiles;
    }

    public static bool IsSdlxliff(string path)
    {
        return ".sdlxliff".Equals(Path.GetExtension(path).ToLowerInvariant(), StringComparison.OrdinalIgnoreCase);
    }
}
