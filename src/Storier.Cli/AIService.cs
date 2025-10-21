namespace Storier.Cli;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public class AIService
{
    private readonly IChatCompletionService _chat;
    private readonly ChatHistory _history;

    public AIService(AppSettings settings)
    {
        var builder = Kernel.CreateBuilder();

        // OpenAI
        builder.AddOpenAIChatCompletion(
            modelId: "gpt-4o-mini",
            apiKey: settings.OpenAI.ApiKey
        );

        //Ollama
        // builder.AddOllamaChatCompletion("llama3");

        var kernel = builder.Build();

        _chat = kernel.GetRequiredService<IChatCompletionService>();
        _history = new ChatHistory();
    }

    public async Task<string> SendMessage(string message)
    {
        _history.AddUserMessage(message);
        var response = await _chat.GetChatMessageContentAsync(_history);
        if (!string.IsNullOrWhiteSpace(response.Content))
        {
            _history.AddAssistantMessage(response.Content);
        }
        return response.Content ?? string.Empty;
    }
}