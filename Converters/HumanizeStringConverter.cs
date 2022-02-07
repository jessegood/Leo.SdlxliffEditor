using System;
using System.Globalization;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.Converters;

public sealed class HumanizeStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var s = ((string)value).Replace('_', ' ');
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
