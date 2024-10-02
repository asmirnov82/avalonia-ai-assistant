using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.Abstractions;
using LLama.Common;

namespace AiAssistant.LlamaSharp.Transformers
{
    /// <summary>
    /// Chat History transformer for Llama 2 family.
    /// https://huggingface.co/blog/llama2#how-to-prompt-llama-2
    /// </summary>
    public class Llama2HistoryTransformer : IHistoryTransform
    {
        public string Name => "Llama2";

        /// <inheritdoc/>
        public IHistoryTransform Clone()
        {
            return new Llama2HistoryTransformer();
        }

        /// <inheritdoc/>
        public string HistoryToText(ChatHistory history)
        {
            //More info on template format for llama2 https://huggingface.co/blog/llama2#how-to-prompt-llama-2
            if (history.Messages.Count == 0)
                return string.Empty;

            var builder = new StringBuilder(64 * history.Messages.Count);

            int i = 0;
            if (history.Messages[i].AuthorRole == AuthorRole.System)
            {
                builder.Append($"<s>[INST] <<SYS>>\n").Append(history.Messages[0].Content.Trim()).Append("\n<</SYS>>\n");
                i++;

                if (history.Messages.Count > 1)
                {
                    builder.Append(history.Messages[1].Content.Trim()).Append(" [/INST]");
                    i++;
                }
            }

            for (; i < history.Messages.Count; i++)
            {
                if (history.Messages[i].AuthorRole == AuthorRole.User)
                {
                    builder.Append("<s>[INST] ").Append(history.Messages[i].Content.Trim()).Append(" [/INST]");
                }
                else
                {
                    builder.Append(' ').Append(history.Messages[i].Content.Trim()).Append(" </s>");
                }
            }
                        
            return builder.ToString();
        }

        /// <inheritdoc/>
        public ChatHistory TextToHistory(AuthorRole role, string text)
        {
            return new ChatHistory([new ChatHistory.Message(role, text)]);
        }
    }
}
