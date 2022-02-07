using Leo.SdlxliffEditor.ViewModels;
using Leo.SdlxliffEditor.Views;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Leo.SdlxliffEditor.Services;

public class ApplicationHostService : IHostedService
{
    private readonly MainWindow mainWindow;
    private readonly MainWindowViewModel mainWindowViewModel;

    public ApplicationHostService(MainWindow mainWindow, MainWindowViewModel mainWindowViewModel)
    {
        this.mainWindow = mainWindow;
        this.mainWindowViewModel = mainWindowViewModel;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        mainWindow.DataContext = mainWindowViewModel;
        mainWindow.Show();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
