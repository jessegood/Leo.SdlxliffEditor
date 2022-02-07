namespace Leo.SdlxliffEditor.TagTypes;

public sealed class TagPairTagType : TagTypeBase
{
    public string BeginPairedTagContents { get; set; }
    public string BeginPairedTagName { get; set; }
    public bool End { get; set; }
    public string EndRevisionId { get; set; }
    public string FormatId { get; set; }
    public bool Start { get; set; }
    public string StartRevisionId { get; set; }
}