using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiAssistant.LlamaSharp
{
    /// <summary>
    /// Settings for Chat Session inference.
    /// </summary>
    public record SessionOptions
    {
        /// <summary>
        /// Prompt for a system role.
        /// </summary>
        public string? SystemInstructions {  get; set; }

        /// <summary>
        /// Name of the custom chat template transform implementation. 
        /// </summary>
        public string? CustomChatTemplate { get; set; }

        /// <summary>
        /// Model context size (n_ctx).
        /// </summary>
        public uint? ContextSize { get; set; }

        ///<summary>
        /// Temperature to apply (higher temperature is more "creative").
        ///</summary>
        public float Temperature { get; set; } = 0.75f;

        /// <summary>
        /// Presence penalty. Number between -2.0 and 2.0.
        /// Positive values penalize new tokens based on whether they appear in the text so far,
        /// increasing the model's likelihood to talk about new topics.
        /// </summary>
        public float PresencePenalty { get; set; }

        /// <summary>
        /// Frequency penalty. Number between -2.0 and 2.0.
        /// Positive values penalize new tokens based on their
        /// existing frequency in the text so far, decreasing the model's likelihood to repeat
        /// the same line verbatim.
        /// </summary>
        public float FrequencyPenalty { get; set; }
    }
}
