using Leo.SdlxliffEditor.ViewModels;
using System;
using System.Collections.Generic;

namespace Leo.SdlxliffEditor.QACheckers;

public static class QACheckerFactory
{
    private static readonly Dictionary<string, Func<bool, QACheckerBase>> qaCheckers = new()
    {
        {
            (string)App.Current.Resources["UnconfirmedSegments"],
            cs => new UnconfirmedSegmentsQAChecker("UnconfirmedSegments", cs)
        },
        {
            (string)App.Current.Resources["EmptyTargetSegments"],
            cs => new EmptyTargetSegmentsQAChecker("EmptyTargetSegments", cs)
        },
        {
            (string)App.Current.Resources["TargetSameAsSource"],
            cs => new TargetSameAsSourceQAChecker("TargetSameAsSource", cs)
        },
        {
            (string)App.Current.Resources["MissingTags"],
            cs => new TagCountMismatchQAChecker("MissingTags", cs)
        },
        {
            (string)App.Current.Resources["TagOrder"],
            cs => new TagOrderQAChecker("TagOrder", cs)
        },
        {
            (string)App.Current.Resources["TagLocation"],
            cs => new TagLocationQAChecker("TagLocation", cs)
        },
    };

    public static QACheckerBase CreateQAChecker(bool isCaseSensitive, QACheckItemViewModel checkItem)
    {
        if (checkItem.Children is not null and { Count: > 0 })
        {
            if (checkItem.Name.Contains("Source"))
            {
                return new SourceInconsistenciesQAChecker("InconsistenciesInSource", isCaseSensitive,
                    checkItem.Children[0].IsChecked == true, checkItem.Children[1].IsChecked == true);
            }
            else
            {
                return new TargetInconsistenciesQAChecker("InconsistenciesInTarget", isCaseSensitive,
                    checkItem.Children[0].IsChecked == true, checkItem.Children[1].IsChecked == true);
            }
        }
        else
        {
            return qaCheckers[checkItem.Name].Invoke(isCaseSensitive);
        }
    }
}
