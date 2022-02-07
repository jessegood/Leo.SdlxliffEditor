using Leo.SdlxliffEditor.Dialogs.ViewModels;
using Leo.SdlxliffEditor.Helpers;
using Leo.SdlxliffEditor.Interfaces;
using Leo.SdlxliffEditor.Services;
using Leo.SdlxliffEditor.ViewModels;
using Leo.SdlxliffEditor.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Threading;

namespace Leo.SdlxliffEditor;

public partial class App : Application
{
    private readonly IHost host;

    public App()
    {
        host = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .Build();
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // Services
        services.AddHostedService<ApplicationHostService>();
        services.AddSingleton<IColorSchemeService, ColorSchemeService>();
        services.AddSingleton<ISdlxliffFileService, SdlxliffFileService>();
        services.AddSingleton<IBatchTaskExecuterService, BatchTaskExecuterService>();
        services.AddSingleton<ISettingsDialogService, SettingsDialogService>();
        services.AddSingleton<IQACheckExecuterService, QACheckExecuterService>();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();

        // Messenger
        services.AddSingleton<IMessenger, StrongReferenceMessenger>();

        // Views
        services.AddSingleton<MainWindow>();

        // View Models
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<DragAndDropFileViewModel>();
        services.AddTransient<EditorViewModel>();
        services.AddTransient<QAViewModel>();
        services.AddTransient<IQASettingsViewModel, QASettingsViewModel>();
        services.AddTransient<BatchTaskViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<IDisplayFilterViewModel, DisplayFilterViewModel>();
        services.AddTransient<ILoadFilesDialogViewModel, LoadFilesDialogViewModel>();
        services.AddTransient<IFindAndReplaceDialogViewModel, FindAndReplaceDialogViewModel>();

        // Helpers
        services.AddTransient<ICommandExceptionHandler, CommandExceptionHandler>();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // TODO: Handle exceptions
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        using (host)
        {
            await host.StopAsync();
        }
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await host.StartAsync();
    }
}