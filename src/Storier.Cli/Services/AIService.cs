namespace Storier.Cli.Services;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Storier.Cli.Models;
using System.IO;

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
        string world = LoadWorld();
        string characters = LoadCharacters();
        string missions = LoadMissionsFromFiles();

        string systemPrompt = $"{systemPromptBase}\n\n### Pamięć\n{memory}\n\n### Świat\n{world}\n\n### Postacie\n{characters}\n\n### Misje\n{missions}";

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
            var files = Directory.GetFiles(Path.Combine("Memory", "missions"), "*.md");
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

    private string LoadMissionsFromFiles()
    {
        try
        {
            var files = Directory.GetFiles(Path.Combine("Memory", "missions"), "*.md");
            var builder = new System.Text.StringBuilder();
            foreach (var file in files)
            {
                builder.AppendLine($"--- {Path.GetFileName(file)} ---");
                builder.AppendLine(File.ReadAllText(file));
                builder.AppendLine();
            }
            return builder.ToString();
        }
        catch
        {
            return "Brak dostępnej pamięci narracyjnej.";
        }
    }

    private string LoadWorld()
    {
        try
        {
            var files = Directory.GetFiles(Path.Combine("Memory", "world"), "*.md");
            var builder = new System.Text.StringBuilder();
            foreach (var file in files)
            {
                builder.AppendLine($"--- {Path.GetFileName(file)} ---");
                builder.AppendLine(File.ReadAllText(file));
                builder.AppendLine();
            }
            return builder.ToString();
        }
        catch
        {
            return "Brak dostępnego opisu świata.";
        }
    }

    private string LoadCharacters()
    {
        try
        {
            var files = Directory.GetFiles(Path.Combine("Memory", "characters"), "*.md");
            var builder = new System.Text.StringBuilder();
            foreach (var file in files)
            {
                builder.AppendLine($"--- {Path.GetFileName(file)} ---");
                builder.AppendLine(File.ReadAllText(file));
                builder.AppendLine();
            }
            return builder.ToString();
        }
        catch
        {
            return "Brak dostępnych informacji o postaciach.";
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