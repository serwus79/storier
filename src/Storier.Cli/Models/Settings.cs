namespace Storier.Cli.Models;

public class AppSettings
{
    public OpenAISettings OpenAI { get; set; } = new();
    public string Memory { get; set; } = string.Empty;
    public string World { get; set; } = string.Empty;
    public string Characters { get; set; } = string.Empty;
}

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
}