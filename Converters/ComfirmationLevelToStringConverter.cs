using Leo.Sdlxliff.Model.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leo.SdlxliffEditor.Converters;

public sealed class ComfirmationLevelToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ConfirmationLevel confirmationLevel)
        {
            switch (confirmationLevel)
            {
                case ConfirmationLevel.Unspecified:
                    return App.Current.Resources["NotTranslated"];

                case ConfirmationLevel.Draft:
                    return App.Current.Resources["Draft"];

                case ConfirmationLevel.Translated:
                    return App.Current.Resources["Translated"];

                case ConfirmationLevel.RejectedTranslation:
                    return App.Current.Resources["TranslationRejected"];

                case ConfirmationLevel.ApprovedTranslation:
                    return App.Current.Resources["TranslationApproved"];

                case ConfirmationLevel.RejectedSignOff:
                    return App.Current.Resources["SignOffRejected"];

                case ConfirmationLevel.ApprovedSignOff:
                    return App.Current.Resources["SignedOff"];
            }
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}