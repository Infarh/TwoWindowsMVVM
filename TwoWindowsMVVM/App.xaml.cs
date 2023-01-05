using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TwoWindowsMVVM.Services;
using TwoWindowsMVVM.Services.Implementation;
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

        services.AddSingleton<IUserDialog, UserDialogService>();

        services.AddTransient(s => new MainWindow { DataContext = s.GetRequiredService<MainWindowViewModel>() });
        services.AddTransient(s => new SecondaryWindow() { DataContext = s.GetRequiredService<SecondaryWindowViewModel>() });

        return services;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Services.GetRequiredService<MainWindow>().Show();
    }
}