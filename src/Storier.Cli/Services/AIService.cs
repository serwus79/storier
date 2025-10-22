namespace Storier.Cli.Services;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Storier.Cli.Models;
using System.IO;
using System.Text;
using OpenAI;

public class AIService
{
    private readonly IChatCompletionService _chat;
    private readonly ChatHistory _history;
    private readonly string _memoryPath;
    private int _totalPromptTokens = 0;
    private int _totalCompletionTokens = 0;
    private decimal _totalCost = 0;
    private int _lastPromptTokens = 0;
    private int _lastCompletionTokens = 0;
    private decimal _lastCost = 0;

    private const decimal InputTokenPrice = 0.15m / 1000000m; // $0.15 per 1M input tokens
    private const decimal OutputTokenPrice = 0.60m / 1000000m; // $0.60 per 1M output tokens

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

    public (int totalPrompt, int totalCompletion, decimal totalCost, int lastPrompt, int lastCompletion, decimal lastCost) GetUsageStats()
    {
        return (_totalPromptTokens, _totalCompletionTokens, _totalCost, _lastPromptTokens, _lastCompletionTokens, _lastCost);
    }

    public async Task<string> SendMessage(string message)
    {
        _history.AddUserMessage(message);
        var response = await _chat.GetChatMessageContentAsync(_history);
        if (response.Metadata != null && response.Metadata.TryGetValue("Usage", out var usageObj) && usageObj != null)
        {
            dynamic usage = usageObj;
            int promptTokens = (int)usage.InputTokenCount;
            int completionTokens = (int)usage.OutputTokenCount;
            decimal cost = (promptTokens * InputTokenPrice) + (completionTokens * OutputTokenPrice);
            _totalPromptTokens += promptTokens;
            _totalCompletionTokens += completionTokens;
            _totalCost += cost;
            _lastPromptTokens = promptTokens;
            _lastCompletionTokens = completionTokens;
            _lastCost = cost;
        }
        if (!string.IsNullOrWhiteSpace(response.Content))
        {
            _history.AddAssistantMessage(response.Content);
        }
        return response.Content ?? string.Empty;
    }
}