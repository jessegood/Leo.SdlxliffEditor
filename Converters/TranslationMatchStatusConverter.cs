using Leo.Sdlxliff;
using Leo.Sdlxliff.Model.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.Converters;

public sealed class TranslationMatchStatusConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is byte percentage)
        {
            var origin = (string)values[1];
            var textMatch = (TextContextMatchLevel)values[2];

            switch (origin)
            {
                case DefaultTranslationOrigin.DocumentMatch: // Perfect match
                    return "PM";
                case DefaultTranslationOrigin.NeuralMachineTranslation:
                    return "NMT";
                case DefaultTranslationOrigin.AutomaticTranslation:
                    return "AT";
            }

            if (IsContextMatch(origin, percentage, textMatch))
            {
                return "CM";
            }

            return percentage.ToString("0\\%", CultureInfo.InvariantCulture);
        }

        return DependencyProperty.UnsetValue;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private bool IsContextMatch(string origin, byte percentage, TextContextMatchLevel textMatch)
    {
        if (origin != DefaultTranslationOrigin.DocumentMatch && percentage >= 100)
        {
            return textMatch == TextContextMatchLevel.SourceAndTarget;
        }

        return false;
    }
}
