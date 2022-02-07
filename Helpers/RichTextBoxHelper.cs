using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.TagTypes;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Leo.SdlxliffEditor.Helpers;

public static class RichTextBoxHelper
{
    private static bool UseFullTagText { get; set; }

    public static void ChangeTagDisplay(RichTextBox rtb, bool useFullTagText)
    {
        UseFullTagText = useFullTagText;

        foreach (var block in rtb.Document.Blocks)
        {
            if (block is Paragraph p)
            {
                foreach (var inline in p.Inlines)
                {
                    if (inline is InlineUIContainer container)
                    {
                        if (container.Tag is PlaceholderTagType placeholder)
                        {
                            ChangeTagText(useFullTagText, container, placeholder.Contents, placeholder.Name);
                        }
                        else if (container.Tag is TagPairTagType pair)
                        {
                            ChangeTagText(useFullTagText, container, pair.BeginPairedTagContents, pair.BeginPairedTagName);
                        }
                        else if (container.Tag is LockContentTagType)
                        {
                            ChangeTagText(useFullTagText, container);
                        }
                    }
                }
            }
        }
    }

    public static void UpdateFlowDocument(RichTextBox rtb, ISegment s)
    {
        var p = new Paragraph();
        foreach (var inline in InlineElementConverter.Convert(s.Contents, UseFullTagText))
        {
            p.Inlines.Add(inline);
        }

        rtb.Document = new FlowDocument(p);
    }

    private static void ChangeTagText(bool useFullTagText, InlineUIContainer container)
    {
        if (container.Child is Border border && border.Child is TextBlock textBlock)
        {
            var inlines = (IList)textBlock.Inlines;
            for (int i = 0; i < inlines.Count; i++)
            {
                if (inlines[i] is InlineUIContainer subcontainer)
                {
                    if (subcontainer.Tag is PlaceholderTagType placeholder)
                    {
                        ChangeTagText(useFullTagText, subcontainer, placeholder.Contents, placeholder.Name);
                    }
                    else if (subcontainer.Tag is TagPairTagType pair)
                    {
                        ChangeTagText(useFullTagText, subcontainer, pair.BeginPairedTagContents, pair.BeginPairedTagName);
                    }
                }
            }
        }
    }

    private static void ChangeTagText(bool useFullTagText, InlineUIContainer container, string contents, string name)
    {
        if (container.Child is Border border && border.Child is TextBlock textBlock)
        {
            textBlock.Inlines.Clear();
            textBlock.Inlines.Add(new Run($"{ (useFullTagText ? contents : $"<{ name }>") }"));
        }
    }
}