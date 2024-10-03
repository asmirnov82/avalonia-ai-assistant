using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using AiAssistant.Models;
using AiAssistant.Utils;

namespace AiAssistant.ViewModels
{
    /// <summary>
    /// ViewModel for Settings.
    /// </summary>
    public sealed class SettingsViewModel : ObservableObject
    {
        private readonly ApplicationModel _applicationModel;

        #region Public properties
        public ICommand BrowseModelCommand { get; }

        public string? ModelPath
        {
            get => _applicationModel.ModelPath;
            set
            {
                _applicationModel.ModelPath = value;
            }
        }

        public int GpuLayerCount
        {
            get => _applicationModel.GpuLayerCount;
            set
            {
                _applicationModel.GpuLayerCount = value;
            }
        }

        public int TotalLayerCount
        {
            get => _applicationModel.TotalLayerCount;
        }
                
        public int ContextSize
        {
            get => _applicationModel.ContextSize;
        }
        #endregion

        public SettingsViewModel(ApplicationModel applicationModel)
        {
            _applicationModel = applicationModel;
            _applicationModel.PropertyChanged += OnApplicationModelPropertyChanged;

            BrowseModelCommand = new AsyncRelayCommand(BrowseForModelAsync);
        }

        private void OnApplicationModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ApplicationModel.ModelPath))
                NotifyPropertyChanged(nameof(ModelPath));
            else if (e.PropertyName == nameof(ApplicationModel.GpuLayerCount))
                NotifyPropertyChanged(nameof(GpuLayerCount));
        }

        private async Task BrowseForModelAsync()
        {            
            if (!(_applicationModel.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop))
                return;

            var storageProvider = desktop.MainWindow?.StorageProvider;

            // Start async operation to open the dialog.
            var files = await storageProvider!.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Model",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("GGUF") { Patterns = ["*.gguf"] }]
            });

            if (files.Count >= 1)
            {
                ModelPath = files[0].Path.LocalPath;
            }
        }
    }
}
