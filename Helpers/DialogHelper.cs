using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;

namespace Leo.SdlxliffEditor.Helpers;

public static class DialogHelper
{
    public static ObservableCollection<string> ShowFolderBrowserDialog()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = (string)App.Current.Resources["SelectFolder"],
            UseDescriptionForTitle = true,
            RootFolder = Environment.SpecialFolder.DesktopDirectory,
            ShowNewFolderButton = false
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var filePaths = new ObservableCollection<string>();

            if (Directory.Exists(dialog.SelectedPath))
            {
                foreach (var sdlxliff in Directory.EnumerateFiles(dialog.SelectedPath, "*.sdlxliff", SearchOption.AllDirectories))
                {
                    if (DragAndDropHelper.IsSdlxliff(sdlxliff))
                    {
                        filePaths.Add(sdlxliff);
                    }
                }
            }

            return filePaths;
        }

        return null;
    }

    public static ObservableCollection<string> ShowOpenFileDialog()
    {
        using var dialog = new OpenFileDialog
        {
            Title = (string)App.Current.Resources["SelectFiles"],
            Filter = "Sdlxliff files (*.sdlxliff)|*.sdlxliff",
            RestoreDirectory = true,
            Multiselect = true
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var filePaths = new ObservableCollection<string>();

            foreach (var fileName in dialog.FileNames)
            {
                if (File.Exists(fileName))
                {
                    if (DragAndDropHelper.IsSdlxliff(fileName))
                    {
                        filePaths.Add(fileName);
                    }
                }
            }

            return filePaths;
        }

        return null;
    }
}