namespace Storier.Cli.Services;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Storier.Cli.Models;
using System.IO;
using System.Linq;

public class AIService
{
    private readonly IChatCompletionService _chat;
    private readonly ChatHistory _history;

    public AIService(IOptions<AppSettings> options)
    {
        var settings = options.Value;
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

        string systemPromptBase = LoadSystemPrompt();
        string memory = LoadMemoryFromFiles();

        string systemPrompt = $"{systemPromptBase}\n\n### Pamięć\n{memory}\n\n### Świat\n{settings.World}\n\n### Postacie\n{settings.Characters}";

        _history.AddSystemMessage(systemPrompt);
    }

    private string LoadSystemPrompt()
    {
        try
        {
            return File.ReadAllText("Memory/system-prompt.md");
        }
        catch
        {
            return "Brak dostępnego system prompt.";
        }
    }

    private string LoadMemoryFromFiles()
    {
        try
        {
            var files = Directory.GetFiles("Memory", "*.md").Where(f => !Path.GetFileName(f).Equals("system-prompt.md", StringComparison.OrdinalIgnoreCase));
            var memoryBuilder = new System.Text.StringBuilder();
            foreach (var file in files)
            {
                memoryBuilder.AppendLine($"--- {Path.GetFileName(file)} ---");
                memoryBuilder.AppendLine(File.ReadAllText(file));
                memoryBuilder.AppendLine();
            }
            return memoryBuilder.ToString();
        }
        catch
        {
            return "Brak dostępnej pamięci narracyjnej.";
        }
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