﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama;
using LLama.Abstractions;
using LLama.Common;
using LLama.Transformers;

namespace AiAssistant.Models
{
    /// <summary>
    /// Wrapper over LLama.ChatSession class, implementing chat session.
    /// </summary>
    public sealed class ChatSession : IDisposable
    {
        private readonly IInferenceParams _inferenceParams;
        private LLamaContext? _context;
        private LLama.ChatSession _session;

        public ChatSession(LLamaWeights model, IContextParams contextParams, IInferenceParams inferenceParams, string? systemInstructions = null, ITextStreamTransform? transform = null)
        {
            _inferenceParams = inferenceParams;

            //Init LLM Session
            _context = model.CreateContext(contextParams);
            var executor = new InteractiveExecutor(_context);
            var chatHistory = new ChatHistory();

            
            if (!String.IsNullOrEmpty(systemInstructions))
                chatHistory.AddMessage(AuthorRole.System, systemInstructions);

            _session = new LLama.ChatSession(executor, chatHistory);


            // Add default template transformer, that uses llama.cpp internal chat template support for most popular models
            // If model template is not supported by llama.cpp - custom transformer is required to format the prompt correctly
            // see: https://github.com/ggerganov/llama.cpp/wiki/Templates-supported-by-llama_chat_apply_template
            _session.WithHistoryTransform(new PromptTemplateTransformer(model, withAssistant: true));

            if (transform != null)
            {
                _session.WithOutputTransform(transform);
            }
        }

        public async IAsyncEnumerable<string> SendAsync(string prompt)
        {
            if (_context == null)
            {
                //TODO
                throw new InvalidOperationException();
            }
            
            await foreach (var text in _session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), _inferenceParams))
            {
                yield return text;
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
            _context = null;
        }
    }
}
