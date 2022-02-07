using Leo.SdlxliffEditor.Interfaces;
using System.Linq;
using System.Windows.Controls;

namespace Leo.SdlxliffEditor.Commands;

public class SegmentPairLockCommand : BaseCommand
{
    public override void Execute(object parameter)
    {
        if (parameter is ListBox lb)
        {
            // If any are unlocked, we lock them all
            if (lb.SelectedItems.Cast<ISegmentPairViewModel>().Any(sp => !sp.IsLocked))
            {
                foreach (ISegmentPairViewModel item in lb.SelectedItems)
                {
                    item.IsLocked = true;
                }
            }
            // Otherwise, we unlock them all
            else
            {
                foreach (ISegmentPairViewModel item in lb.SelectedItems)
                {
                    item.IsLocked = false;
                }
            }
        }
        else if (parameter is DataGrid dg)
        {
            // If any are unlocked, we lock them all
            if (dg.SelectedItems.Cast<IQASegmentPairViewModel>().Any(sp => !sp.IsLocked))
            {
                foreach (IQASegmentPairViewModel item in dg.SelectedItems)
                {
                    item.IsLocked = true;
                }
            }
            // Otherwise, we unlock them all
            else
            {
                foreach (IQASegmentPairViewModel item in dg.SelectedItems)
                {
                    item.IsLocked = false;
                }
            }
        }
    }
}