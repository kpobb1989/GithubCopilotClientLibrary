using GithubApiProxy;
using GithubApiProxy.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Properly display special characters like emojis
Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);

builder.Services.AddGithubCopilotModule();

var app = builder.Build();

var githubCopilotService = app.Services.GetRequiredService<IGithubCopilotClient>();

await githubCopilotService.AuthenticateAsync();
Console.WriteLine("Authentication successful. You can now use the GitHub Copilot API.");

Console.WriteLine("Enter a prompt for GitHub Copilot:");

while (true)
{
    var prompt = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(prompt))
    {
        Console.WriteLine("Prompt cannot be empty. Please enter a valid prompt.");
        continue;
    }

    var text = await githubCopilotService.GetTextCompletionAsync(prompt);

    Console.WriteLine(text);
}