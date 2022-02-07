using System;
using System.Globalization;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.Converters;

public sealed class BooleanToSymbolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "\uE72E" : "\uE785";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return "\uE72E".Equals((string)value, StringComparison.OrdinalIgnoreCase);
    }
}
