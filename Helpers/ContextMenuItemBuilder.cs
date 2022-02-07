using Leo.Sdlxliff.Model.Common;
using Leo.SdlxliffEditor.Commands;
using Leo.SdlxliffEditor.ContextMenus;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Leo.SdlxliffEditor.Helpers;

public static class ContextMenuItemBuilder
{
    public static ObservableCollection<ContextMenuItem> InitializeContentMenuItems()
    {
        List<ContextMenuItem> contextMenuItems = new();

        contextMenuItems.Add(new ContextMenuItem(App.Current.Resources["LockUnlock"] as string,
                                                 new SegmentPairLockCommand()));

        List<ContextMenuItem> contextMenus = new()
        {
            new ContextMenuItem(App.Current.Resources["NotTranslated"] as string,
                                new ChangeConfirmationLevelCommand(ConfirmationLevel.Unspecified)),
            new ContextMenuItem(App.Current.Resources["Draft"] as string,
                                new ChangeConfirmationLevelCommand(ConfirmationLevel.Draft)),
            new ContextMenuItem(App.Current.Resources["Translated"] as string,
                                new ChangeConfirmationLevelCommand(ConfirmationLevel.Translated)),
            new ContextMenuItem(App.Current.Resources["TranslationRejected"] as string,
                                new ChangeConfirmationLevelCommand(ConfirmationLevel.RejectedTranslation)),
            new ContextMenuItem(App.Current.Resources["TranslationApproved"] as string,
                                new ChangeConfirmationLevelCommand(ConfirmationLevel.ApprovedTranslation)),
            new ContextMenuItem(App.Current.Resources["SignOffRejected"] as string,
                                new ChangeConfirmationLevelCommand(ConfirmationLevel.RejectedSignOff)),
            new ContextMenuItem(App.Current.Resources["SignedOff"] as string,
                                new ChangeConfirmationLevelCommand(ConfirmationLevel.ApprovedSignOff))
        };

        contextMenuItems.Add(new ContextMenuItem(App.Current.Resources["Status"] as string, null, contextMenus));

        return new ObservableCollection<ContextMenuItem>(contextMenuItems);
    }
}