using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.ContextMenus;
using Leo.SdlxliffEditor.Dialogs.ViewModels;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.ViewModels;

public class EditorViewModel : ObservableObject, IEditorViewModel
{
    private readonly List<ISdlxliffDocument> documents = new();
    private readonly Dictionary<(int, int), string> fileRanges = new();
    private readonly IFindAndReplaceDialogViewModel findAndReplaceDialogViewModel;
    private readonly IMessenger messenger;
    private readonly ISdlxliffFileService sdlxliffFileService;
    private string currentFile;
    private ObservableCollection<ISegmentPairViewModel> segmentPairs;
    private CollectionViewSource segmentPairsView;
    private SegmentPairViewModel selectedItem;
    private int totalSegmentCount;

    public EditorViewModel(ISdlxliffFileService sdlxliffFileService, IDisplayFilterViewModel displayFilterViewModel,
        IFindAndReplaceDialogViewModel findAndReplaceDialogViewModel, IMessenger messenger)
    {
        this.sdlxliffFileService = sdlxliffFileService;
        DisplayFilterViewModel = displayFilterViewModel;
        this.findAndReplaceDialogViewModel = findAndReplaceDialogViewModel;
        this.messenger = messenger;

        LoadedCommand = new RelayCommand(OnLoaded);

        ContextMenuItems = ContextMenuItemBuilder.InitializeContentMenuItems();
    }

    public event Action<string> FileChanged;


    public event Action<int> FileSelected;


    public event Action<MatchInfo> FindTextExecuted;


    public event Action<MatchInfo, string> ReplaceTextExecuted;

    public ObservableCollection<ContextMenuItem> ContextMenuItems { get; }

    public IDisplayFilterViewModel DisplayFilterViewModel { get; }

    public bool HasUnsavedChanges { get; set; }

    public IRelayCommand LoadedCommand { get; }

    public CollectionViewSource SegmentPairsView
    {
        get => segmentPairsView;
        set => SetProperty(ref segmentPairsView, value);
    }

    public SegmentPairViewModel SelectedItem
    {
        get => selectedItem;
        set => SetProperty(ref selectedItem, value);
    }

    public void Close()
    {
        DisplayFilterViewModel.FileSelected -= OnFileSelected;
        DisplayFilterViewModel.FilterApplied -= OnFilterApplied;
        DisplayFilterViewModel.FilterCleared -= OnFilterCleared;
        SegmentPairsView.Filter -= DisplayFilterViewModel.Filter;

        foreach (var segmentPair in segmentPairs)
        {
            segmentPair.SegmentChanged -= SegmentChanged;
        }
    }

    public void GoToFoundSegment(MatchInfo matchInfo) => FindTextExecuted?.Invoke(matchInfo);

    public void ReplaceText(MatchInfo matchInfo, string replaceText) => ReplaceTextExecuted?.Invoke(matchInfo, replaceText);

    public void ResetChangeTracker()
    {
        foreach (var segmentPair in segmentPairs)
        {
            segmentPair.AcceptChanges();
        }
    }

    public async Task Save() => await sdlxliffFileService.SaveFilesAsync(documents);

    public void Show() => findAndReplaceDialogViewModel.Show(this);

    public void UpdateTitle(double offset)
    {
        // We only worry about updating the title
        // if we have more than one file
        if (fileRanges.Count > 1)
        {
            var fileRange = fileRanges.First(fr => offset >= fr.Key.Item1 && offset <= fr.Key.Item2);
            if (!string.Equals(currentFile, fileRange.Value, StringComparison.CurrentCulture))
            {
                currentFile = fileRange.Value;
                FileChanged?.Invoke(currentFile);
            }
        }
    }

    private async Task<List<ISegmentPairViewModel>> LoadSegmentPairsAsync()
    {
        List<ISegmentPairViewModel> segmentPairs = new();
        int start = 0;
        int rowNumber = 0;
        int documentIndex = 0;

        try
        {
            await foreach (var document in sdlxliffFileService.GetSdlxliffDocumentsAsync())
            {
                start += rowNumber;
                rowNumber = 0;
                documents.Add(document);

                foreach (var tu in document.TranslationUnits.Where(t => t.HasSegmentPairs))
                {
                    foreach (var pair in tu.GetSegmentPairs())
                    {
                        var segmentPair = new SegmentPairViewModel(pair, ContextMenuItems, document);
                        segmentPair.SegmentChanged += SegmentChanged;
                        segmentPairs.Add(segmentPair);
                        rowNumber++;
                    }
                }

                fileRanges.Add((start, start + rowNumber - 1), Path.GetFileName(sdlxliffFileService.FileList[documentIndex++]));
            }
        }
        catch (Exception e)
        {
            MessageBoxHelper.ShowErrorMessage(e);
        }

        return segmentPairs;
    }

    private void OnFileSelected(string fileName)
    {
        var fileRange = fileRanges.FirstOrDefault(p => p.Value.Contains(fileName));

        if (!fileRange.Equals(default(KeyValuePair<(int, int), string>)))
        {
            FileSelected?.Invoke(fileRange.Key.Item1);
        }
    }

    private void OnFilterApplied()
    {
        SegmentPairsView.Filter += DisplayFilterViewModel.Filter;
        DisplayFilterViewModel.SetSegmentCount(SegmentPairsView.View.Cast<object>().Count(), totalSegmentCount);
    }

    private void OnFilterCleared()
    {
        SegmentPairsView.Filter -= DisplayFilterViewModel.Filter;
        DisplayFilterViewModel.SetSegmentCount(totalSegmentCount, totalSegmentCount);
    }

    private async void OnLoaded()
    {
        segmentPairs = new ObservableCollection<ISegmentPairViewModel>(await LoadSegmentPairsAsync());

        ResetChangeTracker();

        // Set the view
        SegmentPairsView = new CollectionViewSource
        {
            Source = segmentPairs
        };

        if (fileRanges.Count > 0)
        {
            // Set the title
            currentFile = fileRanges.First().Value;
            FileChanged?.Invoke(currentFile);

            // Notify the editor is loaded
            messenger.Send<EditorFinishedLoadingRequestMessage>();

            // Hook into filter events and display filter file list
            DisplayFilterViewModel.FilterApplied += OnFilterApplied;
            DisplayFilterViewModel.FilterCleared += OnFilterCleared;
            DisplayFilterViewModel.FileSelected += OnFileSelected;

            // Set total segment count
            totalSegmentCount = SegmentPairsView.View.SourceCollection.Cast<object>().Count();
            DisplayFilterViewModel.SetSegmentCount(totalSegmentCount, totalSegmentCount);

            // Set file list
            DisplayFilterViewModel.SetFileList(sdlxliffFileService.FileList.Select(f => Path.GetFileName(f)));
        }
        else
        {
            // Notify the editor is loaded
            messenger.Send<EditorCloseRequestMessage>();
        }
    }
    private void SegmentChanged()
    {
        HasUnsavedChanges = true;
    }
}