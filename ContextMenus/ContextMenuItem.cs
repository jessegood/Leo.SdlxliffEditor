using Leo.SdlxliffEditor.Commands;
using System.Collections.Generic;

namespace Leo.SdlxliffEditor.ContextMenus;

public class ContextMenuItem
{
    public List<ContextMenuItem> ContextMenuItems { get; }

    public ContextMenuItem(string menuName, BaseCommand command, List<ContextMenuItem> contextMenuItems = null)
    {
        MenuName = menuName;
        Command = command;
        ContextMenuItems = contextMenuItems;
    }

    public BaseCommand Command { get; }
    public string MenuName { get; }
}
