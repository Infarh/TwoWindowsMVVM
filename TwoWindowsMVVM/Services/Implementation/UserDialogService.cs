using Microsoft.Extensions.DependencyInjection;

namespace TwoWindowsMVVM.Services.Implementation;

public class UserDialogService : IUserDialog
{
    private readonly IServiceProvider _Services;

    public UserDialogService(IServiceProvider Services) => _Services = Services;

    private MainWindow? _MainWindow;
    public void OpenMainWindow()
    {
        if (_MainWindow is { } window)
        {
            window.Show();
            return;
        }

        window        =  _Services.GetRequiredService<MainWindow>();
        window.Closed += (_, _) => _MainWindow = null;

        _MainWindow = window;
        window.Show();
    }

    private SecondaryWindow? _SecondaryWindow;
    public void OpenSecondaryWindow()
    {
        if (_SecondaryWindow is { } window)
        {
            window.Show();
            return;
        }

        window        =  _Services.GetRequiredService<SecondaryWindow>();
        window.Closed += (_, _) => _SecondaryWindow = null;

        _SecondaryWindow = window;
        window.Show();
    }
}
