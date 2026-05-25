using AiAssistant.Models;
using AiAssistant.ViewModels;
using AiAssistant.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace AiAssistant
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();

            // Register NLogger
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog();
            });

            services.AddApplicationServices();
            services.AddViewModels();
            services.AddAppSettings("appsettings.json");

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow();
                services.AddSingleton(mainWindow.StorageProvider);

                var serviceProvider = services.BuildServiceProvider();
                var vm = serviceProvider.GetRequiredService<MainWindowViewModel>();

                mainWindow.DataContext = vm;
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
