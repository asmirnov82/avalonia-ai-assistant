using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiAssistant.Models;
using AiAssistant.Utils;

namespace AiAssistant.ViewModels
{
    /// <summary>
    /// ViewModel for Options.
    /// </summary>
    public sealed class OptionsViewModel : ObservableObject
    {
        private readonly ApplicationModel _applicationModel;

        #region Public properties
        public string? SystemInstructions
        {
            get => _applicationModel.SystemInstructions;
            set
            {
                _applicationModel.SystemInstructions = value;
            }
        }

        public float Temperature
        {
            get => _applicationModel.Temperature;
            set
            {
                _applicationModel.Temperature = value;
            }
        }

        public float PresencePenalty
        {
            get => _applicationModel.PresencePenalty;
            set
            {
                _applicationModel.PresencePenalty = value;
            }
        }

        public float FrequencyPenalty
        {
            get => _applicationModel.FrequencyPenalty;
            set
            {
                _applicationModel.FrequencyPenalty = value;
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
            else if (e.PropertyName == nameof(ApplicationModel.PresencePenalty))
                NotifyPropertyChanged(nameof(PresencePenalty));
            else if (e.PropertyName == nameof(ApplicationModel.FrequencyPenalty))
                NotifyPropertyChanged(nameof(FrequencyPenalty));
            else if (e.PropertyName == nameof(ApplicationModel.SystemInstructions))
                NotifyPropertyChanged(nameof(SystemInstructions));
        }
    }
}
