using Leo.Sdlxliff.Interfaces;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.Converters;

public sealed class SegmentToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ISegment s)
        {
            return s.ToString();
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}