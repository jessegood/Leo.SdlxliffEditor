using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.ContextMenus;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Leo.SdlxliffEditor.ViewModels;

public abstract class QACheckResultsViewModelBase : ObservableObject
{
    private bool isLoading;
    private bool isTargetChanged;

    public QACheckResultsViewModelBase(string name, int errorCount = 0)
    {
        Name = name;
        ErrorCount = errorCount;

        ContextMenuItems = ContextMenuItemBuilder.InitializeContentMenuItems();

        CloseTabCommand = new RelayCommand(OnCloseTab);
    }

    public event Action TargetChanged;

    public event Action<QACheckResultsViewModelBase> CheckResultRemoved;

    public IRelayCommand CloseTabCommand { get; }

    public ObservableCollection<ContextMenuItem> ContextMenuItems { get; }

    public int ErrorCount { get; protected set; }

    public bool IsLoading
    {
        get => isLoading;
        set => SetProperty(ref isLoading, value);
    }

    public bool IsTargetChanged
    {
        get => isTargetChanged;
        set
        {
            isTargetChanged = value;
            if (value is true)
            {
                TargetChanged?.Invoke();
            }
        }
    }

    public bool IsVisible => ErrorCount > 0;

    public string Name { get; }

    public IDictionary<string, IList<IQASegmentPairViewModel>> SegmentPairs { get; } = new Dictionary<string, IList<IQASegmentPairViewModel>>();

    public abstract void Add(string fileName, int rowNumber, ISegmentPair segmentPair, ISdlxliffDocument document);

    private void OnCloseTab()
    {
        CheckResultRemoved?.Invoke(this);
    }
}