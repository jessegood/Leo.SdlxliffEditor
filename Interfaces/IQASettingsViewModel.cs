using Leo.SdlxliffEditor.ViewModels;
using System.Collections.Generic;

namespace Leo.SdlxliffEditor.Interfaces;

public interface IQASettingsViewModel
{
    bool CaseSensitive { get; set; }
    bool ExcludeContextMatchSegments { get; set; }
    bool ExcludeEmptySegments { get; set; }
    bool ExcludeLockedSegments { get; set; }
    bool ExcludeOneHundredMatchSegments { get; set; }
    bool ExcludePerfectMatchSegments { get; set; }
    List<QACheckItemViewModel> QACheckItems { get; }
}