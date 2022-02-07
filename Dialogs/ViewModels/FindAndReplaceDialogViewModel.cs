using Leo.Sdlxliff;
using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using Leo.SdlxliffEditor.Dialogs.Views;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Leo.SdlxliffEditor.Dialogs.ViewModels;

public sealed class FindAndReplaceDialogViewModel : ObservableObject, IFindAndReplaceDialogViewModel
{
    private readonly MainWindow mainWindow;
    private int currentMatchIndex;
    private int currentRow = -1;
    private FindAndReplaceDialogView dialog;
    private IEditorViewModel editorViewModel;
    private string findText;
    private bool isCaseSensitive;
    private bool isMatchWholeWord;
    private bool isSkipLockedSegments;
    private bool isTextChanged;
    private List<(string, int)> matches = new();
    private string replaceText;
    private int selectedIndex;
    private bool useRegularExpressions;

    public FindAndReplaceDialogViewModel(MainWindow mainWindow)
    {
        this.mainWindow = mainWindow;

        CloseCommand = new RelayCommand(OnClose);
        FindNextCommand = new RelayCommand(OnFindNext, CanFind);
        FindPreviousCommand = new RelayCommand(OnFindPrevious, CanFind);
        ReplaceCommand = new RelayCommand(OnReplace, CanReplace);
        ReplaceAllCommand = new RelayCommand(OnReplaceAll, CanReplace);
    }

    public RelayCommand CloseCommand { get; }

    public RelayCommand FindNextCommand { get; }

    public RelayCommand FindPreviousCommand { get; }

    public string FindText
    {
        get => findText;
        set
        {
            SetProperty(ref findText, value);
            NotifyCanFindChanged();
            NotifyCanReplaceChanged(true);
        }
    }

    public bool IsCaseSensitive
    {
        get => isCaseSensitive;
        set => SetProperty(ref isCaseSensitive, value);
    }

    public bool IsMatchWholeWord
    {
        get => isMatchWholeWord;
        set => SetProperty(ref isMatchWholeWord, value);
    }

    public bool IsSkipLockedSegments
    {
        get => isSkipLockedSegments;
        set => SetProperty(ref isSkipLockedSegments, value);
    }

    public RelayCommand ReplaceAllCommand { get; }

    public RelayCommand ReplaceCommand { get; }

    public string ReplaceText
    {
        get => replaceText;
        set => SetProperty(ref replaceText, value);
    }

    public int SelectedIndex
    {
        get => selectedIndex;
        set => SetProperty(ref selectedIndex, value);
    }

    public bool UseRegularExpressions
    {
        get => useRegularExpressions;
        set => SetProperty(ref useRegularExpressions, value);
    }

    private bool IsSource => SelectedIndex > 0;

    public void Show(IEditorViewModel editorViewModel)
    {
        this.editorViewModel = editorViewModel;

        dialog = new FindAndReplaceDialogView
        {
            DataContext = this,
            Owner = mainWindow
        };

        dialog.Show();
    }

    private bool CanFind()
    {
        return !string.IsNullOrEmpty(FindText);
    }

    private bool CanReplace()
    {
        return !isTextChanged;
    }

    private List<(string, int)> GetMatches(ISegment segment)
    {
        List<(string, int)> matches = new();
        Dictionary<string, int> counter = new();

        var stack = new Stack<ITranslationUnitContent>();
        stack.Push((ITranslationUnitContent)segment);

        while (stack.Count > 0)
        {
            var content = stack.Pop();

            if (content is IText text)
            {
                if (UseRegularExpressions)
                {
                    foreach (Match match in Regex.Matches(text.Contents, FindText, IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
                    {
                        if (!counter.ContainsKey(match.Value))
                        {
                            counter.Add(match.Value, 0);
                        }

                        counter[match.Value]++;

                        matches.Add((match.Value, counter[match.Value]));
                    }
                }
                else
                {
                    int index = 0;

                    while ((index = text.Contents.IndexOf(FindText, index,
                        IsCaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)) != -1)
                    {
                        index += FindText.Length;
                        if (!counter.ContainsKey(FindText))
                        {
                            counter.Add(FindText, 0);
                        }

                        counter[FindText]++;

                        matches.Add((FindText, counter[FindText]));
                    }
                }
            }

            if (content.HasChildren)
            {
                foreach (var subcontent in ((TranslationUnitContentContainer)content).Contents)
                {
                    stack.Push(subcontent);
                }
            }
        }

        return matches;
    }

    private SegmentSearchSettings GetSettings() => new SegmentSearchSettings()
    {
        // If match whole word is true, we always turn on regular expression searching
        UseRegularExpressions = IsMatchWholeWord || UseRegularExpressions,
        CaseSensitive = IsCaseSensitive
    };

    private void NotifyCanFindChanged()
    {
        FindNextCommand.NotifyCanExecuteChanged();
        FindPreviousCommand.NotifyCanExecuteChanged();
    }

    private void NotifyCanReplaceChanged(bool isTextChanged = false)
    {
        this.isTextChanged = isTextChanged;
        ReplaceCommand.NotifyCanExecuteChanged();
        ReplaceAllCommand.NotifyCanExecuteChanged();
    }

    private void OnClose()
    {
        dialog?.Close();
    }

    private void OnFindNext()
    {
        // As long as the count is greater than occurrence
        // We use the stored matches in the current row
        if (matches.Count > currentMatchIndex)
        {
            editorViewModel.GoToFoundSegment(new MatchInfo(currentRow, matches[currentMatchIndex].Item1, matches[currentMatchIndex].Item2, IsSource));
            ++currentMatchIndex;
            return;
        }

        bool found = SearchNext(editorViewModel.SegmentPairsView.Source as ObservableCollection<ISegmentPairViewModel>);

        if (!found)
        {
            MessageBoxHelper.ShowMessage($"No{ (currentRow > -1 ? " more" : "")  } matches for { FindText } found.");
            ResetState();
        }
        else
        {
            NotifyCanReplaceChanged();
        }
    }

    private void OnFindPrevious()
    {
        if (currentMatchIndex > 1)
        {
            currentMatchIndex -= 2;
            editorViewModel.GoToFoundSegment(new MatchInfo(currentRow, matches[currentMatchIndex].Item1, matches[currentMatchIndex].Item2, IsSource));
            ++currentMatchIndex;
            return;
        }

        bool found = SearchPrevious(editorViewModel.SegmentPairsView.Source as ObservableCollection<ISegmentPairViewModel>);

        if (!found)
        {
            MessageBoxHelper.ShowMessage($"No{ (currentRow > -1 ? " more" : "")  } matches for { FindText } found.");
            ResetState();
        }
        else
        {
            NotifyCanReplaceChanged();
        }
    }

    private void OnReplace()
    {
        editorViewModel.ReplaceText(new MatchInfo(currentRow, matches[currentMatchIndex - 1].Item1, matches[currentMatchIndex - 1].Item2, IsSource), ReplaceText);
        OnFindNext();
    }

    private void OnReplaceAll()
    {
        ResetState();
        int count = 0;
        bool found = false;

        do
        {
            if (matches.Count > currentMatchIndex - 1)
            {
                editorViewModel.ReplaceText(new MatchInfo(currentRow, matches[currentMatchIndex - 1].Item1, matches[currentMatchIndex - 1].Item2, IsSource), ReplaceText);
                ++currentMatchIndex;
                ++count;
                continue;
            }

            found = SearchNext(editorViewModel.SegmentPairsView.Source as ObservableCollection<ISegmentPairViewModel>);
        } while (found);

        MessageBoxHelper.ShowMessage($"{ count } matches have been replaced.");
    }

    private string PrepareSearchString()
    {
        if (IsMatchWholeWord)
        {
            return $@"\b{FindText}\b";
        }
        else
        {
            return FindText;
        }
    }

    private void ResetState()
    {
        currentRow = -1;
        matches.Clear();
    }

    private bool SearchNext(ObservableCollection<ISegmentPairViewModel> segmentPairs)
    {
        var start = currentRow > -1 ? currentRow + 1 : 0;
        var search = IsSkipLockedSegments ? segmentPairs.Skip(start).Where(sp => sp.IsLocked == false) : segmentPairs.Skip(start);
        var segmentPair = IsSource ? search.FirstOrDefault(sp => SegmentSearchHelper.Search(PrepareSearchString(), null, sp.SegmentPair, GetSettings()))
            : search.FirstOrDefault(sp => SegmentSearchHelper.Search(null, PrepareSearchString(), sp.SegmentPair, GetSettings()));

        if (segmentPair is not null)
        {
            SetMatchInfo(segmentPair);
            return true;
        }

        return false;
    }

    private bool SearchPrevious(ObservableCollection<ISegmentPairViewModel> segmentPairs)
    {
        var start = currentRow > -1 ? currentRow - 1 : segmentPairs.Count - 1;

        if (IsSource)
        {
            for (int i = start; i >= 0; --i)
            {
                var segmentPair = segmentPairs[i];
                if (!ShouldSkip(segmentPair))
                {
                    if (SegmentSearchHelper.Search(PrepareSearchString(), null, segmentPair.SegmentPair, GetSettings()))
                    {
                        SetMatchInfo(segmentPair, true);
                        return true;
                    }
                }
            }
        }
        else
        {
            for (int i = start; i >= 0; --i)
            {
                var segmentPair = segmentPairs[i];
                if (!ShouldSkip(segmentPair))
                {
                    if (SegmentSearchHelper.Search(null, PrepareSearchString(), segmentPair.SegmentPair, GetSettings()))
                    {
                        SetMatchInfo(segmentPair, true);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void SetMatchInfo(ISegmentPairViewModel segmentPair, bool isFindPrevious = false)
    {
        matches = GetMatches(IsSource ? segmentPair.SourceSegment : segmentPair.TargetSegment);
        currentMatchIndex = isFindPrevious ? matches.Count : 1;
        currentRow = segmentPair.RowNumber - 1;
        editorViewModel.GoToFoundSegment(new MatchInfo(currentRow, isFindPrevious ? matches.Last().Item1 : matches.First().Item1,
             isFindPrevious ? matches.Last().Item2 : matches.First().Item2, IsSource));
    }

    private bool ShouldSkip(ISegmentPairViewModel segmentPair)
    {
        return IsSkipLockedSegments && segmentPair.IsLocked == true;
    }
}