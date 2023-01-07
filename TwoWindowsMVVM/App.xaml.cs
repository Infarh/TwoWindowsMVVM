using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TwoWindowsMVVM.Services;
using TwoWindowsMVVM.Services.Implementations;
using TwoWindowsMVVM.ViewModels;

namespace TwoWindowsMVVM;

public partial class App
{
    private static IServiceProvider? _Services;

    public static IServiceProvider Services => _Services ??= InitializeServices().BuildServiceProvider();

    private static IServiceCollection InitializeServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<SecondaryWindowViewModel>();

        services.AddSingleton<IUserDialog, UserDialogServices>();
        services.AddSingleton<IMessageBus, MessageBusService>();

        services.AddTransient(
            s =>
            {
                var model  = s.GetRequiredService<MainWindowViewModel>();
                var window = new MainWindow { DataContext = model };
                model.DialogComplete += (_, _) => window.Close();

                return window;
            });

        services.AddTransient(
            s =>
            {
                var model  = s.GetRequiredService<SecondaryWindowViewModel>();
                var window = new SecondaryWindow { DataContext = model };
                model.DialogComplete += (_, _) => window.Close();

                return window;
            });

        return services;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Services.GetRequiredService<IUserDialog>().OpenMainWindow();
    }
}