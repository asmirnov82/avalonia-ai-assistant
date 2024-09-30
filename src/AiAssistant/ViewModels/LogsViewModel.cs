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
    /// ViewModel for application status log.
    /// </summary>
    public sealed class LogsViewModel : ObservableObject
    {
        private string? _text;
        public string? Text
        {
            get => _text;
            set
            {
                _text = value;
                NotifyPropertyChanged();
            }
        }

        public LogsViewModel (ApplicationStatusLog statusLog)
        {
            statusLog.PropertyChanged += OnStatusLogPropertyChanged;
        }

        private void OnStatusLogPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ApplicationStatusLog.Text))
            {
                Text = ((ApplicationStatusLog?)sender)?.Text;
                
            }
        }
    }
}
