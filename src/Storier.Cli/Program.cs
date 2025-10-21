
using Microsoft.Extensions.Configuration;
using Storier.Cli.Models;
using Storier.Cli.Services;


var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

var settings = configuration.Get<AppSettings>() ?? throw new InvalidOperationException("Failed to load settings.");

var aiService = new AIService(settings!);

Console.WriteLine("AI Narrator ready. Type your message:");

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    var response = await aiService.SendMessage(input);
    Console.WriteLine(response);
}