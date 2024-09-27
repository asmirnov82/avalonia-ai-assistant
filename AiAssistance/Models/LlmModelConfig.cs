using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistant.Models
{
    /// <summary>
    /// A class for reading model settings from app config file.
    /// </summary>
    public sealed class LlmModelConfig
    {
        //Model params
        public string? ModelName { get; set; }
        public string? ModelPath { get; set; }
        public int GpuLayerCount { get; set; }
        public int TotalLayerCount { get; set; }
        public int ContextSize { get; set; }

        //Inference params
        public float Temperature { get; set; }

        public string? AntiPrompts { get; set; }
        public string? SkippedOutput { get; set; }
        public string? SystemInstructions { get; set; }
    }
}
