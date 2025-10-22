namespace Storier.Cli.Models;

public class AppSettings
{
    public OpenAISettings OpenAI { get; set; } = new();
    public string MemoryPath { get; set; } = "Memory";
    public string ModelId { get; set; } = "gpt-4o-mini";
}

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
}

public static class ModelPricing
{
    private static readonly Dictionary<string, (decimal Input, decimal CachedInput, decimal Output)> _prices = new()
    {
        ["gpt-5"] = (1.25m, 0.125m, 10.00m),
        ["gpt-5-mini"] = (0.25m, 0.025m, 2.00m),
        ["gpt-5-nano"] = (0.05m, 0.005m, 0.40m),
        ["gpt-5-chat-latest"] = (1.25m, 0.125m, 10.00m),
        ["gpt-5-codex"] = (1.25m, 0.125m, 10.00m),
        ["gpt-5-pro"] = (15.00m, 0m, 120.00m),
        ["gpt-4.1"] = (2.00m, 0.50m, 8.00m),
        ["gpt-4.1-mini"] = (0.40m, 0.10m, 1.60m),
        ["gpt-4.1-nano"] = (0.10m, 0.025m, 0.40m),
        ["gpt-4o"] = (2.50m, 1.25m, 10.00m),
        ["gpt-4o-2024-05-13"] = (5.00m, 0m, 15.00m),
        ["gpt-4o-mini"] = (0.15m, 0.075m, 0.60m),
        ["gpt-realtime"] = (4.00m, 0.40m, 16.00m),
        ["gpt-realtime-mini"] = (0.60m, 0.06m, 2.40m),
        ["gpt-4o-realtime-preview"] = (5.00m, 2.50m, 20.00m),
        ["gpt-4o-mini-realtime-preview"] = (0.60m, 0.30m, 2.40m),
        ["gpt-audio"] = (2.50m, 0m, 10.00m),
        ["gpt-audio-mini"] = (0.60m, 0m, 2.40m),
        ["gpt-4o-audio-preview"] = (2.50m, 0m, 10.00m),
        ["gpt-4o-mini-audio-preview"] = (0.15m, 0m, 0.60m),
        ["o1"] = (15.00m, 7.50m, 60.00m),
        ["o1-pro"] = (150.00m, 0m, 600.00m),
        ["o3-pro"] = (20.00m, 0m, 80.00m),
        ["o3"] = (2.00m, 0.50m, 8.00m),
        ["o3-deep-research"] = (10.00m, 2.50m, 40.00m),
        ["o4-mini"] = (1.10m, 0.275m, 4.40m),
        ["o4-mini-deep-research"] = (2.00m, 0.50m, 8.00m),
        ["o3-mini"] = (1.10m, 0.55m, 4.40m),
        ["o1-mini"] = (1.10m, 0.55m, 4.40m),
        ["codex-mini-latest"] = (1.50m, 0.375m, 6.00m),
        ["gpt-5-search-api"] = (1.25m, 0.125m, 10.00m),
        ["gpt-4o-mini-search-preview"] = (0.15m, 0m, 0.60m),
        ["gpt-4o-search-preview"] = (2.50m, 0m, 10.00m),
        ["computer-use-preview"] = (3.00m, 0m, 12.00m),
        ["gpt-image-1"] = (5.00m, 1.25m, 0m),
        ["gpt-image-1-mini"] = (2.00m, 0.20m, 0m),
    };

    public static (decimal Input, decimal CachedInput, decimal Output) GetPrices(string modelId)
    {
        return _prices.TryGetValue(modelId, out var prices) ? prices : (0.15m, 0.075m, 0.60m); // default gpt-4o-mini
    }
}