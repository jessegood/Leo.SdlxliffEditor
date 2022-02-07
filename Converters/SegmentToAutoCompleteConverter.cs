using Leo.Sdlxliff.Interfaces;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.TagTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace Leo.SdlxliffEditor.Converters;

public sealed class SegmentToAutoCompleteConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ISegment segment)
        {
            var list = CreateTagList(segment);
            if (list is null)
            {
                // Error occurred
                return DependencyProperty.UnsetValue;
            }
            else if (list.Count() > 0)
            {
                // List contains tags
                return list;
            }
            else
            {
                // No tags in segments, so return null
                return null;
            }
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static IEnumerable<TextBlock> CreateTagList(ISegment segment)
    {
        List<TextBlock> list = new();
        Stack<TextBlock> stack = new();

        foreach (var inline in InlineElementConverter.Convert(segment.Contents, false))
        {
            if (inline is InlineUIContainer)
            {
                switch (inline.Tag)
                {
                    case LockContentTagType:
                    case PlaceholderTagType:
                    case StructureTagType:
                        list.Add(new TextBlock(inline));
                        break;

                    case TagPairTagType:
                        stack.Push(new TextBlock(inline));
                        break;

                    case null:
                        if (stack.Count > 0)
                        {
                            stack.Peek().Inlines.Add(inline);
                            list.Add(stack.Pop());
                        }
                        else
                        {
                            return null;
                        }
                        break;

                    default:
                        return null;
                }
            }
        }

        return list;
    }
}