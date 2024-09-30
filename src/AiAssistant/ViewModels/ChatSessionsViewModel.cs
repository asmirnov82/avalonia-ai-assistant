using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AiAssistant.Models;
using AiAssistant.Utils;

namespace AiAssistant.ViewModels
{
    /// <summary>
    /// ViewModel for a list of user chat sessions.
    /// </summary>
    public sealed class ChatSessionsViewModel : ObservableObject
    {
        private readonly ApplicationModel _applicationModel;
        private readonly ObservableCollection<ChatSessionViewModel> _chatSessions;
                
        #region Public properties
        public ObservableCollection<ChatSessionViewModel> ChatSessions => _chatSessions;

        private bool _isModelLoading = false;
        public bool IsModelLoading
        {
            get { return _isModelLoading; }
            private set
            {
                if (_isModelLoading != value)
                {
                    _isModelLoading = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _modelLoadingProgress;
        public int ModelLoadingProgress
        {
            get { return _modelLoadingProgress; }
            private set
            {
                _modelLoadingProgress = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand NewChatCommand { get; }

        public bool IsNewChatButtonEnabled => true;
        #endregion

        public ChatSessionsViewModel(ApplicationModel model)
        {
            _applicationModel = model;
            _chatSessions = new ObservableCollection<ChatSessionViewModel>();

            NewChatCommand = new AsyncRelayCommand(NewChatAsync);
        }
                
        private async Task NewChatAsync()
        {
            if (!_applicationModel.IsModelLoaded)
            {
                IsModelLoading = true;

                await _applicationModel.LoadModelWeightsAsync(new Progress<float>(x => ModelLoadingProgress = (int)(x * 100)));

                IsModelLoading = false;
            }

            var session = _applicationModel.StartChatSession();

            if (session == null)
                //TODO show error
                return;

            var chatSession = new ChatSessionViewModel(session, "Chat " + (_chatSessions.Count + 1));
            chatSession.OnClose += ChatSessionOnClose;
            _chatSessions.Add(chatSession);
            SelectedTabIndex = _chatSessions.Count - 1;
        }

        private void ChatSessionOnClose(object? sender, EventArgs e)
        {
            if (sender != null)
                _chatSessions.Remove((ChatSessionViewModel)sender);

            if (_chatSessions.Count == 0)
            {
                _applicationModel.UnloadLanguageModel();
                ModelLoadingProgress = 0;
            }
        }
    }
}
