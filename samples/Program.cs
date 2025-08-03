using GithubApiProxy;
using GithubApiProxy.Abstractions;
using GithubApiProxy.Abstractions.HttpClients;
using GithubApiProxy.HttpClients.GithubApi;
using GithubApiProxy.HttpClients.GithubCopilot;
using GithubApiProxy.HttpClients.GithubWeb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Properly display special characters like emojis
Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);

builder.Services.AddHttpClient(nameof(GithubWebHttpClient), client =>
{
    client.BaseAddress = new Uri("https://github.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient(nameof(GithubApiHttpClient), client =>
{
    client.BaseAddress = new Uri("https://api.github.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", AppSettings.UserAgent);
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", AppSettings.GithubApiVersion);
});

builder.Services.AddHttpClient(nameof(GithubCopilotHttpClient), client =>
{
    client.BaseAddress = new Uri("https://api.individual.githubcopilot.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("Editor-Version", AppSettings.EditorVersion);
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2025-04-01");
});

// Register http clients
builder.Services.AddSingleton<IGithubWebHttpClient, GithubWebHttpClient>();
builder.Services.AddSingleton<IGithubApiHttpClient, GithubApiHttpClient>();
builder.Services.AddSingleton<IGithubCopilotHttpClient, GithubCopilotHttpClient>();
builder.Services.AddSingleton<IGithubCopilotClient, GithubCopilotClient>();

var app = builder.Build();

var githubCopilotService = app.Services.GetRequiredService<IGithubCopilotClient>();
await githubCopilotService.AuthenticateAsync();
Console.WriteLine("Authentication successful. You can now use the GitHub Copilot API.");

Console.WriteLine("Enter a prompt for GitHub Copilot:");

while (true)
{
    var prompt = Console.ReadLine();

    var text = await githubCopilotService.GetTextCompletionAsync(prompt!);

    Console.WriteLine(text);
}