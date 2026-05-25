using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AiAssistance.Models;
using AiAssistant.LlamaSharp;
using AiAssistant.Common;

namespace AiAssistant.Models
{
    /// <summary>
    /// Application model that contains main app business logic.
    /// </summary>
    public sealed class ApplicationModel : ObservableObject, IApplicationModel
    {
        private readonly ApplicationStatusLog _statusLog = new ApplicationStatusLog();
        private readonly string? _customChatTemplate;

        private readonly LlmModel _llm;
                
        public ApplicationStatusLog StatusLog => _statusLog;
        public ChatSession? ChatSession;

        #region Public properties
        private bool _isModelLoaded = false;
        public bool IsModelLoaded
        {
            get => _isModelLoaded;
            private set
            {
                _isModelLoaded = value;
                NotifyPropertyChanged();
            }
        }

        private string? _modelPath = null;
        public string? ModelPath
        {
            get => _modelPath;
            set
            {
                _modelPath = value;
                NotifyPropertyChanged();
            }
        }

        private int _gpuLayerCount;
        public int GpuLayerCount
        {
            get => _gpuLayerCount;
            set
            {
                _gpuLayerCount = value;
                NotifyPropertyChanged();
            }
        }

        private int _totalLayerCount;
        public int TotalLayerCount
        {
            get => _totalLayerCount;
        }

        private int _contextSize;
        public int ContextSize
        {
            get => _contextSize;
        }

        private float _temperature;
        public float Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                NotifyPropertyChanged();
            }
        }

        private float _presencePenalty;
        public float PresencePenalty
        {
            get => _presencePenalty;
            set
            {
                _presencePenalty = value;
                NotifyPropertyChanged();
            }
        }

        private float _frequencyPenalty;
        public float FrequencyPenalty
        {
            get => _frequencyPenalty;
            set
            {
                _frequencyPenalty = value;
                NotifyPropertyChanged();
            }
        }

        private string? _systemInstructions;
        public string? SystemInstructions
        {
            get => _systemInstructions;
            set
            {
                _systemInstructions = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public ApplicationModel(LlmConfig llmConfig, InferenceConfig inferenceConfig)
        {
            //Configure llama.cpp native lib                      
            try
            {
                LlmModel.InitNativeLibs(_statusLog.Log);
            }
            catch (Exception ex)
            {
                _statusLog.Log(LogLevel.Error, ex.Message);
            }
                        
            _modelPath = llmConfig!.Path + llmConfig.FileName;
            _gpuLayerCount = llmConfig.GpuLayerCount;
            _totalLayerCount = llmConfig.TotalLayerCount;
            _contextSize = llmConfig.ContextSize;
            _customChatTemplate = llmConfig.CustomHistoryTransformer;
                        
            _systemInstructions = inferenceConfig!.SystemInstructions;
            _temperature = inferenceConfig.Temperature;
            _presencePenalty = inferenceConfig.PresencePenalty;
            _frequencyPenalty = inferenceConfig.FrequencyPenalty;

            _llm = new LlmModel();
        }

        public async Task<bool> LoadModelAsync(IProgress<float>? progressReporter = null)
        {
            if (_isModelLoaded)
            {
                _statusLog.Log(LogLevel.Error, "Model is already loaded");
                return false;
            }

            try
            {                
                await _llm.LoadModel(_modelPath!, _gpuLayerCount, progressReporter: progressReporter);
                IsModelLoaded = true;
            }
            catch (Exception ex)
            {
                _statusLog.Log(LogLevel.Error, ex.Message);
            }

            return true;
        }

        public void UnloadModel()
        {
            //unload model
            _llm.Unload();
            IsModelLoaded = false;
        }

        public ChatSession? StartChatSession()
        {
            if (_llm == null)
                return null;

            return _llm.StartChatSession(new SessionOptions()
            {
                SystemInstructions = _systemInstructions,
                ContextSize = (uint)_contextSize,
                Temperature = _temperature,
                FrequencyPenalty = _frequencyPenalty,
                PresencePenalty = _presencePenalty,
                CustomChatTemplate = _customChatTemplate
            });
        }
    }
}
