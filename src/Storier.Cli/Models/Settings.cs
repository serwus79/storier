namespace Storier.Cli.Models;

public class AppSettings
{
    public OpenAISettings OpenAI { get; set; } = new();
}

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
}