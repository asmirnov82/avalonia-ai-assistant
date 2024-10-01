using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using Microsoft.Extensions.Logging;

namespace AiAssistant.LlamaSharp
{
    /// <summary>
    /// A class that implements LLM Model abstraction over LLama.LLamaWeights object.
    /// </summary>
    public class LlmModel
    {
        private LLamaWeights? _llm;

        public static void InitNativeLibs(Action<LogLevel, string> action)
        {
            //Configure llama.cpp native lib                      
            NativeLibraryConfig
                .All
                .WithLogCallback((level, message) =>
                {
                    action.Invoke(ConvertLLamaLogLevel(level), message);
                });
        }

        public async Task LoadModel(string path, int gpuLayerCount = 0, IProgress<float>? progressReporter = null)
        {
            if (_llm != null)
            {
                //TODO
                throw new Exception();
            }

            var modelParams = new ModelParams(path)
            {
                Seed = 1337,
                GpuLayerCount = gpuLayerCount
            };

            //Load model weights
            _llm = await LLamaWeights.LoadFromFileAsync(modelParams, progressReporter: progressReporter);
        }

        public void Unload()
        {
            _llm?.Dispose();
            _llm = null;
        }

        public ChatSession? StartChatSession(SessionOptions options)
        {
            if (_llm == null)
                return null;

            //Define context  and inference params
            var contextParams = new ModelParams("")
            {
                ContextSize = options.ContextSize
            };

            var samplingPipeline = new DefaultSamplingPipeline()
            {
                Temperature = options.Temperature,
                AlphaPresence = options.PresencePenalty,
                AlphaFrequency = options.FrequencyPenalty
            };
            
            //Define inference params
            var inferenceParams = new InferenceParams()
            {
                AntiPrompts = [_llm.Tokens.EndOfTurnToken ?? "User:"],
                SamplingPipeline = samplingPipeline
            };

            return new ChatSession(_llm,
                contextParams,
                inferenceParams,
                options.SystemInstructions,
                new LLamaTransforms.KeywordTextOutputStreamTransform([_llm.Tokens.EndOfTurnToken ?? "User:", "�"], redundancyLength: 5));
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
