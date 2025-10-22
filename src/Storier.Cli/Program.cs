
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storier.Cli.Models;
using Storier.Cli.Services;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

var services = new ServiceCollection();
services.Configure<AppSettings>(configuration);
services.AddSingleton<AIService>();
var serviceProvider = services.BuildServiceProvider();

var aiService = serviceProvider.GetRequiredService<AIService>();

Console.WriteLine("AI Narrator ready. Type your message:");

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    var response = await aiService.SendMessage(input);
    Console.WriteLine(response);
    var (totalPrompt, totalCompletion, totalCost, lastPrompt, lastCompletion, lastCost) = aiService.GetUsageStats();
    Console.WriteLine("-----------------------------------");
    Console.WriteLine($"Last: {lastPrompt} prompt + {lastCompletion} completion tokens, ${lastCost:F4}");
    Console.WriteLine($"Total: {totalPrompt} prompt + {totalCompletion} completion tokens, ${totalCost:F4}");
}