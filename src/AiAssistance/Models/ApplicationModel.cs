using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using AIAssistant.Utils;
using LLama;
using LLama.Common;
using LLama.Native;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AiAssistance.Models;

namespace AIAssistant.Models
{
    /// <summary>
    /// Application model that contains main app business logic.
    /// </summary>
    public sealed class ApplicationModel : ObservableObject
    {
        private readonly IApplicationLifetime? _applicationLifetime;
        private readonly ApplicationStatusLog _statusLog = new ApplicationStatusLog();
        private readonly IConfiguration _config;
        
        private LLamaWeights? _llm;

        public IApplicationLifetime? ApplicationLifetime => _applicationLifetime;
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

        public ApplicationModel(IApplicationLifetime? applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;

            //Configure llama.cpp native lib                      
            try
            {
                NativeLibraryConfig
                    .All
                    .WithLogCallback((level, message) =>
                    {
                        _statusLog.Log(ConvertLLamaLogLevel(level), message);
                    });
            }
            catch (Exception ex)
            {
                _statusLog.Log(LogLevel.Error, ex.Message);
            }

            //Read app config from appsettings.json file
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            //Read and define model params
            var modelConfig = _config.GetSection("ModelParams").Get<LlmConfig>();
                        
            _modelPath = modelConfig!.ModelPath + modelConfig.ModelName;
            _gpuLayerCount = modelConfig.GpuLayerCount;
            _totalLayerCount = modelConfig.TotalLayerCount;
            _contextSize = modelConfig.ContextSize;

            //Read and define inference params
            var inferenceConfig = _config.GetSection("InferenceParams").Get<InferenceConfig>();

            _temperature = inferenceConfig!.Temperature;
            _systemInstructions = inferenceConfig.SystemInstructions;
        }

        public async Task<bool> LoadModelWeightsAsync(IProgress<float>? progressReporter = null)
        {
            if (_llm != null)
            {
                _statusLog.Log(LogLevel.Error, "Model is already loaded");
                return false;
            }

            try
            {
                var modelParams = new ModelParams(_modelPath!)
                {
                    Seed = 1337,
                    GpuLayerCount = _gpuLayerCount
                };

                //Load model weights
                _llm = await LLamaWeights.LoadFromFileAsync(modelParams, progressReporter: progressReporter);
                IsModelLoaded = true;
            }
            catch (Exception ex)
            {
                _statusLog.Log(LogLevel.Error, ex.Message);
            }

            return true;
        }

        public void UnloadLanguageModel()
        {
            //unload model
            _llm?.Dispose();
            _llm = null;
            IsModelLoaded = false;
        }

        public ChatSession? StartChatSession()
        {
            if (_llm == null)
                return null;

            //Define context  and inference params
            var contextParams = new ModelParams(_modelPath!)
            {
                ContextSize = (uint)_contextSize
            };

            //Define inference params
            var inferenceParams = new InferenceParams()
            {
                AntiPrompts = [_llm.Tokens.EndOfTurnToken!],
                Temperature = _temperature
            };

            return new ChatSession(_llm,
                contextParams,
                inferenceParams,
                _systemInstructions,
                new LLamaTransforms.KeywordTextOutputStreamTransform([_llm.Tokens.EndOfTurnToken!, "�"], redundancyLength: 5));
        }

        private static LogLevel ConvertLLamaLogLevel(LLamaLogLevel level)
        {
            switch (level)
            {
                case LLamaLogLevel.Debug:
                    return LogLevel.Debug;
                case LLamaLogLevel.Info:
                    return LogLevel.Information;
                case LLamaLogLevel.Warning:
                    return LogLevel.Warning;
                case LLamaLogLevel.Error:
                    return LogLevel.Error;

                default:
                    return LogLevel.None;
            }
        }
    }
}
