using Leo.SdlxliffEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.Interfaces;

public interface IDisplayFilterViewModel
{
    event Action FilterApplied;

    event Action FilterCleared;
    event Action<string> FileSelected;

    List<FilterAttributesViewModel> FilterAttributes { get; }
    bool IsCaseSensitive { get; set; }
    bool SearchInTags { get; set; }
    string Source { get; set; }
    string Target { get; set; }
    bool UseRegularExpression { get; set; }
    void Filter(object sender, FilterEventArgs e);
    void SetFileList(IEnumerable<string> fileList);
    void SetSegmentCount(int count, int total);
}
