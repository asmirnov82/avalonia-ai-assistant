using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiAssistant.Utils;
using Microsoft.Extensions.Logging;

namespace AiAssistant.Models
{
    /// <summary>
    /// Storage for application log.
    /// </summary>
    public sealed class ApplicationStatusLog : ObservableObject
    {
        private string _log = string.Empty;

        public string Text
        {
            get => _log;
            set 
            {
                _log = value;
                NotifyPropertyChanged();
            }
        }
                
        public void Log(LogLevel level, string message)
        {
            Text += level + ": " + message + "\n";
        }
    }
}
