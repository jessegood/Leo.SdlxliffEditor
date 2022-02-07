using System.Collections.Generic;

namespace Leo.SdlxliffEditor.ViewModels;

public class FilterAttributesViewModel : CheckBoxViewModelBase<FilterAttributesViewModel>
{
    public FilterAttributesViewModel(string name)
        : base(name)
    {
    }

    public static List<FilterAttributesViewModel> CreateFilterAttributes()
    {
        return new List<FilterAttributesViewModel>()
            {
                new FilterAttributesViewModel(App.Current.Resources["Status"] as string)
                {
                    Children = new()
                    {
                        new FilterAttributesViewModel(App.Current.Resources["NotTranslated"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["Draft"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["Translated"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["TranslationRejected"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["TranslationApproved"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["SignOffRejected"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["SignedOff"] as string),
                    }
                },
                new FilterAttributesViewModel(App.Current.Resources["Origin"] as string)
                {
                    Children = new()
                    {
                        new FilterAttributesViewModel(App.Current.Resources["PerfectMatch"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["ContextMatch"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["AutomatedTranslation"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["HundredPercentMatch"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["FuzzyMatch"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["Interactive"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["CopiedFromSource"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["AutoPropagation"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["NeuralMachineTranslation"] as string),
                    }
                },
                new FilterAttributesViewModel(App.Current.Resources["Repetitions"] as string)
                {
                    Children = new()
                    {
                        new FilterAttributesViewModel(App.Current.Resources["All"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["FirstOccurences"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["ExcludeFirstOccurences"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["UniqueOccurences"] as string)
                    }
                },
                new FilterAttributesViewModel(App.Current.Resources["Review"] as string)
                {
                    Children = new()
                    {
                        new FilterAttributesViewModel(App.Current.Resources["WithComments"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["WithTrackChanges"] as string),
                    }
                },
                new FilterAttributesViewModel(App.Current.Resources["Locking"] as string)
                {
                    Children = new()
                    {
                        new FilterAttributesViewModel(App.Current.Resources["Locked"] as string),
                        new FilterAttributesViewModel(App.Current.Resources["Unlocked"] as string),
                    }
                },
            };
    }
}
