using Microsoft.Toolkit.Mvvm.Input;

namespace Leo.SdlxliffEditor.Interfaces;

public interface ICommandExceptionHandler
{
    void RegisterCommands(params IAsyncRelayCommand[] commands);
}