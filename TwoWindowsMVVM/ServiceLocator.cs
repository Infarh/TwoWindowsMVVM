using Microsoft.Extensions.DependencyInjection;
using TwoWindowsMVVM.ViewModels;

namespace TwoWindowsMVVM;

public class ServiceLocator
{
    public MainWindowViewModel MainModel => App.Services.GetRequiredService<MainWindowViewModel>();

    public SecondaryWindowViewModel SecondaryModel => App.Services.GetRequiredService<SecondaryWindowViewModel>();
}
