using Leo.Sdlxliff.Model.Common;

namespace Leo.SdlxliffEditor.TagTypes;

// Types uses to store information in the "Tag" property of UIElements
// These types need to be public and require a parameterless default construtor in order to be serializable

public abstract class TagTypeBase
{
    public string Id { get; set; }
}