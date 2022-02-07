using Leo.Sdlxliff.Model.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.Converters;

public sealed class ConfirmationLevelToSymbolConverter : IValueConverter
{
    private const string document = "\uE8A5";
    private const string pencil = "\uED63";
    private const string magnifyingGlass = "\uE721";
    private const string like = "\uE8E1";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ConfirmationLevel confirmationLevel)
        {
            switch (confirmationLevel)
            {
                case ConfirmationLevel.Unspecified:
                    return document;

                case ConfirmationLevel.Draft:
                case ConfirmationLevel.Translated:
                    return pencil;

                case ConfirmationLevel.RejectedTranslation:
                case ConfirmationLevel.ApprovedTranslation:
                    return magnifyingGlass;

                case ConfirmationLevel.RejectedSignOff:
                case ConfirmationLevel.ApprovedSignOff:
                    return like;
            }
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
