using System.Collections.Generic;

namespace Leo.SdlxliffEditor.ViewModels;

public class QACheckItemViewModel : CheckBoxViewModelBase<QACheckItemViewModel>
{
    public QACheckItemViewModel(string name)
        : base(name)
    {
    }

    public static List<QACheckItemViewModel> CreateQACheckItems()
    {
        return new List<QACheckItemViewModel>()
            {
                new QACheckItemViewModel(App.Current.Resources["UntranslatedSegments"] as string)
                {
                    Children = new List<QACheckItemViewModel>()
                    {
                        new QACheckItemViewModel(App.Current.Resources["UnconfirmedSegments"] as string),
                        new QACheckItemViewModel(App.Current.Resources["EmptyTargetSegments"] as string)
                    }
                },
                new QACheckItemViewModel(App.Current.Resources["Inconsistencies"] as string)
                {
                    Children = new List<QACheckItemViewModel>()
                    {
                        new QACheckItemViewModel(App.Current.Resources["InconsistenciesInTarget"] as string)
                        {
                            Children = new List<QACheckItemViewModel>()
                            {
                                new QACheckItemViewModel(App.Current.Resources["IgnoreNumbers"] as string),
                                new QACheckItemViewModel(App.Current.Resources["IgnorePunctuation"] as string)
                            }
                        },
                        new QACheckItemViewModel(App.Current.Resources["InconsistenciesInSource"] as string)
                        {
                            Children = new List<QACheckItemViewModel>()
                            {
                                new QACheckItemViewModel(App.Current.Resources["IgnoreNumbers"] as string),
                                new QACheckItemViewModel(App.Current.Resources["IgnorePunctuation"] as string)
                            }
                        },
                        new QACheckItemViewModel(App.Current.Resources["TargetSameAsSource"] as string)
                    }
                },
                new QACheckItemViewModel(App.Current.Resources["TagMismatch"] as string)
                {
                    Children = new List<QACheckItemViewModel>()
                    {
                        new QACheckItemViewModel(App.Current.Resources["MissingTags"] as string),
                        new QACheckItemViewModel(App.Current.Resources["TagOrder"] as string),
                        new QACheckItemViewModel(App.Current.Resources["TagLocation"] as string),
                    }
                }
            };
    }
}
