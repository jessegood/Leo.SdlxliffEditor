using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using System.Collections.Generic;

namespace Leo.SdlxliffEditor.QACheckers;

public abstract class TagQAChecker : QACheckerBase
{
    protected TagQAChecker(string name, bool isCaseSensitive)
        : base(name, isCaseSensitive)
    {
    }

    protected IEnumerable<ITranslationUnitContent> GetTags(ISegment segment)
    {
        List<ITranslationUnitContent> tags = new();
        foreach (var content in segment.AllContent())
        {
            switch (content.ContentType)
            {
                case TranslationUnitContentType.LockedContent:
                case TranslationUnitContentType.Placeholder:
                case TranslationUnitContentType.PairedTag:
                case TranslationUnitContentType.StructureTag:
                    tags.Add(content);
                    break;

                default:
                    break;
            }
        }

        return tags;
    }
}