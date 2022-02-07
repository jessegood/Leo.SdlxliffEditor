using AdonisUI.Controls;
using System;
using System.Collections;
using System.Text;

namespace Leo.SdlxliffEditor.Helpers;

public static class MessageBoxHelper
{
    public static MessageBoxResult ShowConfirmationMessage(System.Windows.Window owner, string message, MessageBoxButton button)
    {
        return MessageBox.Show(owner, message, (string)App.Current.Resources["Confirmation"], button, MessageBoxImage.Question);
    }

    public static void ShowErrorMessage(Exception e)
    {
        StringBuilder stringBuilder = new();

        if (e.Data.Count > 0)
        {
            foreach (DictionaryEntry pair in e.Data)
            {
                stringBuilder.Append($"{ pair.Key }: { pair.Value + Environment.NewLine }");
            }
        }

        _ = MessageBox.Show(
                $"{ (stringBuilder.Length > 0 ? stringBuilder.ToString() : String.Empty) + Environment.NewLine + e.ToString() }",
                (string)App.Current.Resources["Error"],
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
    }

    public static void ShowMessage(string message)
    {
        _ = MessageBox.Show(message, (string)App.Current.Resources["Information"], MessageBoxButton.OK, MessageBoxImage.Information);
    }
}