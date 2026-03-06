using System.IO;
using System.Windows;
using BastionBuilder.Application.Interfaces;
using BastionBuilder.Application.ViewModels;
using BastionBuilder.Export;
using BastionBuilder.Persistence.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BastionBuilder.App;

public partial class App
{
    private ServiceProvider? _serviceProvider;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BastionBuilder",
            "bastion.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        IServiceCollection services = new ServiceCollection();
        services.AddSqlitePersistence($"Data Source={dbPath}");
        services.AddSingleton<IExportService, JsonExportService>();
        services.AddTransient<MainViewModel>();

        _serviceProvider = services.BuildServiceProvider();

        // Ensure the database schema is current
        await PersistenceServiceExtensions.InitialiseDatabaseAsync(_serviceProvider);

        var mainWindow = new Views.MainWindow
        {
            DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
