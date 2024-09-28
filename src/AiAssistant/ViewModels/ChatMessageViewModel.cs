using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIAssistant.Utils;

namespace AIAssistant.ViewModels
{
    /// <summary>
    /// ViewModel for Chat Message.
    /// </summary>
    public sealed class ChatMessageViewModel : ObservableObject
    {
        #region Public properties
        private string _content = String.Empty;
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isUserInput;
        public bool IsUserInput
        {
            get { return _isUserInput; }
            set
            {
                _isUserInput = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public ChatMessageViewModel(string content, bool isUserInput)
        {
            _content = content;
            _isUserInput = isUserInput;
        }
    }
}
