
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

var builder = Kernel.CreateBuilder();

// OpenAI
builder.AddOpenAIChatCompletion(
    modelId: "gpt-4o-mini",
    apiKey: configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key not found in configuration.")
);

//Ollama
// builder.AddOllamaChatCompletion("llama3");

var kernel = builder.Build();

var chat = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();

Console.WriteLine("AI Narrator ready. Type your message:");

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    history.AddUserMessage(input);
    var response = await chat.GetChatMessageContentAsync(history);
    Console.WriteLine(response.Content);
    if (!string.IsNullOrWhiteSpace(response.Content))
    {
        history.AddAssistantMessage(response.Content);
    }
}