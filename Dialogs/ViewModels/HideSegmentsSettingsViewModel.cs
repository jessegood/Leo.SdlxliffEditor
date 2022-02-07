namespace Leo.SdlxliffEditor.Dialogs.ViewModels;

public class HideSegmentsSettingsViewModel : SettingsViewModelBase
{
    private bool contextMatchSegments;
    private bool lockedSegments;
    private bool oneHundredMatchSegments;
    private bool perfectMatchSegments;
    private string title = (string)App.Current.Resources["SegmentHideBatchTaskSettings"];

    public override string Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }

    public bool ContextMatchSegments
    {
        get => contextMatchSegments;
        set => SetProperty(ref contextMatchSegments, value);
    }

    public bool LockedSegments
    {
        get => lockedSegments;
        set => SetProperty(ref lockedSegments, value);
    }

    public bool OneHundredMatchSegments
    {
        get => oneHundredMatchSegments;
        set => SetProperty(ref oneHundredMatchSegments, value);
    }

    public bool PerfectMatchSegments
    {
        get => perfectMatchSegments;
        set => SetProperty(ref perfectMatchSegments, value);
    }
}
