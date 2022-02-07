using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace Leo.SdlxliffEditor.ViewModels;

public class QASettingsViewModel : ObservableObject, IQASettingsViewModel
{
    private bool caseSensitive;

    private bool excludeContextMatchSegments;
    private bool excludeEmptySegments;
    private bool excludeLockedSegments;
    private bool excludeOneHundredMatchSegments;
    private bool excludePerfectMatchSegments;

    public QASettingsViewModel()
    {
        QACheckItems = QACheckItemViewModel.CreateQACheckItems();
    }

    public bool CaseSensitive
    {
        get => caseSensitive;
        set => SetProperty(ref caseSensitive, value);
    }

    public bool ExcludeContextMatchSegments
    {
        get => excludeContextMatchSegments;
        set => SetProperty(ref excludeContextMatchSegments, value);
    }

    public bool ExcludeEmptySegments
    {
        get => excludeEmptySegments;
        set => SetProperty(ref excludeEmptySegments, value);
    }

    public bool ExcludeLockedSegments
    {
        get => excludeLockedSegments;
        set => SetProperty(ref excludeLockedSegments, value);
    }

    public bool ExcludeOneHundredMatchSegments
    {
        get => excludeOneHundredMatchSegments;
        set => SetProperty(ref excludeOneHundredMatchSegments, value);
    }

    public bool ExcludePerfectMatchSegments
    {
        get => excludePerfectMatchSegments;
        set => SetProperty(ref excludePerfectMatchSegments, value);
    }

    public List<QACheckItemViewModel> QACheckItems { get; }
}