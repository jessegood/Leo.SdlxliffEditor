using Leo.SdlxliffEditor.Interfaces;
using Microsoft.Toolkit.Mvvm.Input;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Leo.SdlxliffEditor.Helpers;

public class CommandExceptionHandler : ICommandExceptionHandler
{
    public void RegisterCommands(params IAsyncRelayCommand[] commands)
    {
        foreach (var command in commands)
        {
            command.PropertyChanged += CommandPropertyChanged;
        }
    }

    private void CommandPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IAsyncRelayCommand.ExecutionTask) &&
            sender is IAsyncRelayCommand command &&
            command.ExecutionTask is Task task && task.IsFaulted)
        {
            MessageBoxHelper.ShowErrorMessage(task.Exception);
        }
    }
}