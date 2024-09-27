using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIAssistant.Models;
using AIAssistant.Utils;

namespace AIAssistant.ViewModels
{
    /// <summary>
    /// ViewModel for Options.
    /// </summary>
    public sealed class OptionsViewModel : ObservableObject
    {
        private readonly ApplicationModel _applicationModel;

        #region Public properties
        public float Temperature
        {
            get => _applicationModel.Temperature;
            set
            {
                _applicationModel.Temperature = value;
            }
        }

        public string? SystemInstructions
        {
            get => _applicationModel.SystemInstructions;
            set
            {
                _applicationModel.SystemInstructions = value;
            }
        }
        #endregion

        public OptionsViewModel(ApplicationModel applicationModel)
        {
            _applicationModel = applicationModel;
            _applicationModel.PropertyChanged += OnApplicationModelPropertyChanged;
        }

        private void OnApplicationModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ApplicationModel.Temperature))
                NotifyPropertyChanged(nameof(Temperature));
            else if (e.PropertyName == nameof(ApplicationModel.SystemInstructions))
                NotifyPropertyChanged(nameof(SystemInstructions));
        }
    }
}
