using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AiAssistant.Models;
using AiAssistant.ViewModels;
using AiAssistant.Views;

namespace AiAssistant
{
    public partial class App : Application
    {
        private MainWindowViewModel? _mainViewModel;
        private ApplicationModel? _applicationModel;
                
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            _applicationModel = new ApplicationModel(ApplicationLifetime);

            //Create main window view model over application model
            _mainViewModel = new MainWindowViewModel(_applicationModel);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = _mainViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();

            _applicationModel.StatusLog.Log(Microsoft.Extensions.Logging.LogLevel.Information, "Application successfully started");
        }
    }
}
