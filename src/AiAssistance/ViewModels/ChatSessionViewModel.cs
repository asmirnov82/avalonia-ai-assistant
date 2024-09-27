using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AIAssistant.Utils;
using ChatSession = AIAssistant.Models.ChatSession;

namespace AIAssistant.ViewModels
{
    /// <summary>
    /// ViewModel for a user Chat Session.
    /// </summary>
    public sealed class ChatSessionViewModel : ObservableObject
    {
        private readonly ObservableCollection<ChatMessageViewModel> _messages = new ObservableCollection<ChatMessageViewModel>();
        private readonly ChatSession _chatSession;

        #region Public properties
        private string? _header;
        public string? Header => _header;

        public ObservableCollection<ChatMessageViewModel> Messages => _messages;

        public ICommand CloseCommand { get; }
        public ICommand SendCommand { get; }

        public event EventHandler? OnClose;

        private string _userRequest = string.Empty;
        public string UserRequest
        {
            get => _userRequest;
            set
            {
                if (_userRequest != value)
                {
                    _userRequest = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isGeneratingResponse = false;
        public bool IsGeneratingResponse
        {
            get { return _isGeneratingResponse; }
            private set
            {
                if (_isGeneratingResponse != value)
                {
                    _isGeneratingResponse = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        public ChatSessionViewModel(ChatSession chatSession, string? header = null)
        {
            _header = header;
            _chatSession = chatSession;

            SendCommand = new AsyncRelayCommand(SendAsync);
            CloseCommand = new RelayCommand(Close);
        }

        private async Task SendAsync()
        {
            Messages.Add(new ChatMessageViewModel(_userRequest, true));
            var prompt = _userRequest;
            UserRequest = string.Empty;

            //Generate response
            IsGeneratingResponse = true;
            var currentResult = new ChatMessageViewModel("", false);
            Messages.Add(currentResult);
            
            await foreach (var text in _chatSession.SendAsync(prompt))
            {
                currentResult.Content += text;
            }
            IsGeneratingResponse = false;
        }

        private void Close()
        {
            _chatSession.Dispose();

            OnClose?.Invoke(this, new EventArgs());
        }
    }
}
