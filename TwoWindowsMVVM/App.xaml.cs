using Microsoft.Extensions.DependencyInjection;
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

        return services;
    }
}