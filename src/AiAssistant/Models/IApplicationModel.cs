using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiAssistant.LlamaSharp;
using Avalonia.Platform.Storage;

namespace AiAssistant.Models
{
    public interface IApplicationModel : INotifyPropertyChanged
    {
        bool IsModelLoaded { get; }
        int TotalLayerCount { get; }
        int ContextSize { get; }

        string? ModelPath { get; set; }
        int GpuLayerCount { get; set; }
        
        float Temperature { get; set; }
        float PresencePenalty { get; set; }
        float FrequencyPenalty { get; set; }
        string? SystemInstructions { get; set; }

        public ApplicationStatusLog StatusLog { get; }

        Task<bool> LoadModelAsync(IProgress<float>? progressReporter = null);
        ChatSession? StartChatSession();
        void UnloadModel();
    }
}
