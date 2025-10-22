namespace Storier.Cli.Services;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Storier.Cli.Models;
using System.IO;
using System.Text;

public class AIService
{
    private readonly IChatCompletionService _chat;
    private readonly ChatHistory _history;
    private readonly string _memoryPath;

    public AIService(IOptions<AppSettings> options)
    {
        var settings = options.Value;
        _memoryPath = settings.MemoryPath;
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
        string player = LoadPlayer();
        string missions = LoadMissionsFromFiles();

        string systemPrompt = $"{systemPromptBase}\n\n### Pamięć\n{memory}\n\n### Świat\n{world}\n\n### Postacie\n{characters}\n\n### Gracz\n{player}\n\n### Misje\n{missions}";

        _history.AddSystemMessage(systemPrompt);
    }

    private void LoadFilesFromFolder(StringBuilder builder, string folderName, string context = "")
    {
        try
        {
            var files = Directory.GetFiles(Path.Combine(_memoryPath, folderName), "*.md");
            foreach (var file in files)
            {
                string prefix = string.IsNullOrEmpty(context) ? "" : $"{context}: ";
                builder.AppendLine($"--- {prefix}{Path.GetFileName(file)} ---");
                builder.AppendLine(File.ReadAllText(file));
                builder.AppendLine();
            }
        }
        catch
        {
            // Log error if needed
        }
    }

    private string LoadSystemPrompt()
    {
        try
        {
            return File.ReadAllText(Path.Combine(_memoryPath, "system-prompt.md"));
        }
        catch
        {
            return "Brak dostępnego system prompt.";
        }
    }

    private string LoadMemoryFromFiles()
    {
        var builder = new System.Text.StringBuilder();
        LoadFilesFromFolder(builder, "memory");
        return builder.ToString();
    }

    private string LoadMissionsFromFiles()
    {
        var builder = new System.Text.StringBuilder();
        LoadFilesFromFolder(builder, "missions");
        return builder.ToString();
    }

    private string LoadWorld()
    {
        var builder = new System.Text.StringBuilder();
        LoadFilesFromFolder(builder, "world");
        return builder.ToString();
    }

    private string LoadCharacters()
    {
        var builder = new System.Text.StringBuilder();
        LoadFilesFromFolder(builder, "characters");
        LoadFilesFromFolder(builder, Path.Combine("characters", "padre"), "Player");
        return builder.ToString();
    }

    private string LoadPlayer()
    {
        var builder = new System.Text.StringBuilder();
        LoadFilesFromFolder(builder, "player", "Player");
        return builder.ToString();
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