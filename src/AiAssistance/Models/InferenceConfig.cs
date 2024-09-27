using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiAssistance.Models
{
    /// <summary>
    /// A class for reading model settings from app config file.
    /// </summary>
    public sealed class InferenceConfig
    {
        //Inference params
        public float Temperature { get; set; }
        public string? SystemInstructions { get; set; }
    }
}
