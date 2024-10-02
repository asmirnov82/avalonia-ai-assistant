using AiAssistant.LlamaSharp.Transformers;
using LLama.Abstractions;
using LLama.Common;
using LLama.Transformers;
using static LLama.Common.ChatHistory;

namespace AiAssistant.LlamaSharp.Tests
{
    // Description of expected chat template https://huggingface.co/blog/llama2#how-to-prompt-llama-2
    public class Llama2HistoryTransformerTests
    {
        private readonly IHistoryTransform Transformer;

        public Llama2HistoryTransformerTests()
        {
            Transformer = new Llama2HistoryTransformer();
        }

        [Fact]
        public void HistoryToText_OneUserPrompt_EncodesCorrectly()
        {
            const string userPrompt = "Test user prompt.";
                        
            var template = Transformer.HistoryToText(new ChatHistory()
            {
                Messages = [new Message(AuthorRole.User, $" {userPrompt} ")]
            });

            const string expected = "<s>[INST] " +
                                    $"{userPrompt} [/INST]";

            Assert.Equal(expected, template);
        }

        [Fact]
        public void HistoryToText_SystemInstructionsAndOneUserPrompt_EncodesCorrectly()
        {
            const string systemInstructions = "Test system instructions.";
            const string userPrompt = "Test user prompt.";
            
            var template = Transformer.HistoryToText(new ChatHistory()
            {
                Messages = [
                    new Message(AuthorRole.System, $" {systemInstructions} "),
                    new Message(AuthorRole.User, $" {userPrompt} ")]
            });

            const string expected = "<s>[INST] <<SYS>>\n" +
                                    $"{systemInstructions}\n" +
                                    "<</SYS>>\n" +
                                    $"{userPrompt} [/INST]";

            Assert.Equal(expected, template);
        }

        [Fact]
        public void HistoryToText_SeveralUserPrompts_EncodesCorrectly()
        {
            const string userPrompt_1 = "Test user prompt 1.";
            const string userPrompt_2 = "Test user prompt 2.";

            const string assistantAnswer = "Test user prompt 1.";

            var template = Transformer.HistoryToText(new ChatHistory()
            {
                Messages = [
                    new Message(AuthorRole.User, $" {userPrompt_1} "),
                    new Message(AuthorRole.Assistant, assistantAnswer),
                    new Message(AuthorRole.User, $" {userPrompt_2} ")]
            });

            const string expected = "<s>[INST] " +
                                    $"{userPrompt_1} [/INST] {assistantAnswer} </s><s>[INST] {userPrompt_2} [/INST]";

            Assert.Equal(expected, template);
        }

        [Fact]
        public void HistoryToText_SystemInstructionsAndSeveralUserPrompts_EncodesCorrectly()
        {
            const string systemInstructions = "Test system instructions.";
            const string userPrompt_1 = "Test user prompt 1.";
            const string userPrompt_2 = "Test user prompt 2.";

            const string assistantAnswer = "Test user prompt 1.";

            var template = Transformer.HistoryToText(new ChatHistory()
            {
                Messages = [
                    new Message(AuthorRole.System, $" {systemInstructions} "),
                    new Message(AuthorRole.User, $" {userPrompt_1} "),
                    new Message(AuthorRole.Assistant, assistantAnswer),
                    new Message(AuthorRole.User, $" {userPrompt_2} ")]
            });

            const string expected = "<s>[INST] <<SYS>>\n" +
                                    $"{systemInstructions}\n" +
                                    "<</SYS>>\n" +
                                    $"{userPrompt_1} [/INST] {assistantAnswer} </s><s>[INST] {userPrompt_2} [/INST]";

            Assert.Equal(expected, template);
        }
    }
}
