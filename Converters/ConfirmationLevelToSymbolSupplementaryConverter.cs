using Leo.Sdlxliff.Model.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.Converters;

public sealed class ConfirmationLevelToSymbolSupplementaryConverter : IValueConverter
{
    private const string checkMark = "\uF1D8";
    private const string xMark = "\uEA83";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ConfirmationLevel confirmationLevel)
        {
            switch (confirmationLevel)
            {
                case ConfirmationLevel.Unspecified:
                case ConfirmationLevel.Draft:
                    break;

                case ConfirmationLevel.RejectedTranslation:
                case ConfirmationLevel.RejectedSignOff:
                    return xMark;

                case ConfirmationLevel.Translated:
                case ConfirmationLevel.ApprovedTranslation:
                case ConfirmationLevel.ApprovedSignOff:
                    return checkMark;
            }
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
