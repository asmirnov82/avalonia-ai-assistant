using Avalonia;
using Avalonia.Styling;
using AIAssistant.Models;
using AIAssistant.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AIAssistant.ViewModels
{
    /// <summary>
    /// ViewModel for a main application window.
    /// </summary>
    public sealed class MainWindowViewModel : ObservableObject
    {
        private readonly ApplicationModel _applicationModel;

        private readonly ChatSessionsViewModel _chatSessionsViewModel;
        private readonly OptionsViewModel _optionsViewModel;
        private readonly LogsViewModel _logsViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        #region Public properties
        public ICommand ShowChatsCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowOptionsCommand { get; }
        public ICommand ShowLogsCommand { get; }
        public ICommand ChangeThemeCommand { get; }

        private ObservableObject _activeView;
        public ObservableObject ActiveView
        {
            get { return _activeView; }
            set
            {
                if (_activeView != value)
                {
                    _activeView = value;

                    UpdateViewModelState();
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isChatSessionsButtonEnabled;
        public bool IsChatSessionsButtonEnabled
        {
            get => _isChatSessionsButtonEnabled;
            private set
            {
                if (_isChatSessionsButtonEnabled != value)
                {
                    _isChatSessionsButtonEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isSettingsButtonEnabled;
        public bool IsSettingsButtonEnabled
        {
            get => _isSettingsButtonEnabled;
            private set
            {
                if (_isSettingsButtonEnabled != value)
                {
                    _isSettingsButtonEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isOptionsButtonEnabled;
        public bool IsOptionsButtonEnabled
        {
            get => _isOptionsButtonEnabled;
            private set
            {
                if (_isOptionsButtonEnabled != value)
                {
                    _isOptionsButtonEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isLogsButtonEnabled;
        public bool IsLogsButtonEnabled
        {
            get => _isLogsButtonEnabled;
            private set
            {
                if (_isLogsButtonEnabled != value)
                {
                    _isLogsButtonEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        public MainWindowViewModel(ApplicationModel model)
        {
            _applicationModel = model;
            _applicationModel.PropertyChanged += OnApplicationModelPropertyChanged;

            _chatSessionsViewModel = new ChatSessionsViewModel(_applicationModel);
            _optionsViewModel = new OptionsViewModel(_applicationModel);
            _settingsViewModel = new SettingsViewModel(_applicationModel);
            _logsViewModel = new LogsViewModel(_applicationModel.StatusLog);

            _activeView = _chatSessionsViewModel;

            ShowChatsCommand = new RelayCommand(() => ActiveView = _chatSessionsViewModel);
            ShowSettingsCommand = new RelayCommand(() => ActiveView = _settingsViewModel);
            ShowOptionsCommand = new RelayCommand(() => ActiveView = _optionsViewModel);
            ShowLogsCommand = new RelayCommand(() => ActiveView = _logsViewModel);
            ChangeThemeCommand = new RelayCommand(ToggleApplicationScheme);

            UpdateViewModelState();
        }

        private void OnApplicationModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ApplicationModel.IsModelLoaded))
            {
                UpdateViewModelState();
            }
        }

        private void UpdateViewModelState ()
        {
            IsChatSessionsButtonEnabled  = ActiveView != _chatSessionsViewModel;
            IsSettingsButtonEnabled = ActiveView != _settingsViewModel && !_applicationModel.IsModelLoaded;
            IsOptionsButtonEnabled = ActiveView != _optionsViewModel;
            IsLogsButtonEnabled = ActiveView != _logsViewModel;
        }

        private static void ToggleApplicationScheme()
        {
            if (Application.Current != null)
            {
                Application.Current.RequestedThemeVariant =
                    Application.Current.RequestedThemeVariant == ThemeVariant.Light
                        ? ThemeVariant.Dark
                        : ThemeVariant.Light;
            }
        }
    }
}
