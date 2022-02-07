using Leo.Sdlxliff.Model.Common;
using Leo.SdlxliffEditor.Interfaces;
using System.Windows.Controls;

namespace Leo.SdlxliffEditor.Commands;

public class ChangeConfirmationLevelCommand : BaseCommand
{
    private readonly ConfirmationLevel confirmationLevel;

    public ChangeConfirmationLevelCommand(ConfirmationLevel confirmationLevel)
    {
        this.confirmationLevel = confirmationLevel;
    }

    public override void Execute(object parameter)
    {
        if (parameter is ListBox lb)
        {
            foreach (ISegmentPairViewModel item in lb.SelectedItems)
            {
                item.ConfirmationLevel = confirmationLevel;
            }
        }
        else if (parameter is DataGrid dg)
        {
            foreach (IQASegmentPairViewModel item in dg.SelectedItems)
            {
                item.ConfirmationLevel = confirmationLevel;
            }
        }
    }
}