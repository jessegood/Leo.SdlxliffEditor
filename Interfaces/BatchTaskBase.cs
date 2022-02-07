using Leo.Sdlxliff.Interfaces;

namespace Leo.SdlxliffEditor.Interfaces;

public interface IBatchTask
{
    void ProcessSegmentPair(ISegmentPair segmentPair);

    string Name { get; set; }

    string Description { get; set; }
}
