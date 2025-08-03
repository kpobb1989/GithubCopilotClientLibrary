using GithubApiProxy;
using GithubApiProxy.HttpClients.GithubApi;
using GithubApiProxy.HttpClients.GithubCopilot;
using GithubApiProxy.HttpClients.GithubWeb;
using GithubApiProxy.HttpClients.GithubWeb.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient(nameof(GithubWebHttpClient), client =>
{
    client.BaseAddress = new Uri("https://github.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient(nameof(GithubApiHttpClient), client =>
{
    client.BaseAddress = new Uri("https://api.github.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("Editor-Version", AppSettings.EditorVersion);
    client.DefaultRequestHeaders.Add("Editor-Plugin-Version", AppSettings.EditorPluginVersion);
    client.DefaultRequestHeaders.Add("User-Agent", AppSettings.UserAgent);
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", AppSettings.GithubApiVersion);
});

builder.Services.AddHttpClient(nameof(GithubCopilotHttpClient), client =>
{
    client.BaseAddress = new Uri("https://api.individual.githubcopilot.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("Copilot-Integration-Id", "vscode-chat");
    client.DefaultRequestHeaders.Add("Editor-Version", AppSettings.EditorVersion);
    client.DefaultRequestHeaders.Add("User-Agent", AppSettings.UserAgent);
    client.DefaultRequestHeaders.Add("Openai-Intent", "conversation-panel");
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2025-04-01");
    client.DefaultRequestHeaders.Add("X-Request-Id", Guid.NewGuid().ToString());
    client.DefaultRequestHeaders.Add("X-Vscode-User-Agent-Library-Version", "electron-fetch");
});

// Register services
builder.Services.AddSingleton<GithubWebHttpClient>();
builder.Services.AddSingleton<GithubApiHttpClient>();
builder.Services.AddSingleton<GithubCopilotHttpClient>();

var app = builder.Build();

AccessTokenDto? accessToken = null;

var githubTokenPath = Path.Combine(AppContext.BaseDirectory, AppSettings.GithubTokenFileName);

if (File.Exists(githubTokenPath))
{
    using var fileStream = File.OpenRead(githubTokenPath);

    accessToken = await JsonSerializer.DeserializeAsync<AccessTokenDto>(fileStream);
}

if (accessToken == null)
{
    var githubWebHttpClient = app.Services.GetRequiredService<GithubWebHttpClient>();
    var deviceCode = await githubWebHttpClient.GetDeviceCodeAsync();

    Console.WriteLine($"Please enter the code {deviceCode.UserCode} in ${deviceCode.VerificationUri}");

    Process.Start(new ProcessStartInfo
    {
        FileName = deviceCode.VerificationUri,
        UseShellExecute = true
    });

    accessToken = await githubWebHttpClient.GetAccessTokenAsync(deviceCode.DeviceCode, deviceCode.Interval);

    using var fileStream = File.Create(githubTokenPath);
    await JsonSerializer.SerializeAsync(
        fileStream,
        accessToken,
        new JsonSerializerOptions { WriteIndented = true }
    );
}

using var githubApiHttpClient = app.Services.GetRequiredService<GithubApiHttpClient>();
githubApiHttpClient.SetAccessToken(accessToken.AccessToken);

//var userResponse = await githubApiHttpClient.GetUserAsync(); 
//var copilotToken = await githubApiHttpClient.GetCopilotTokenAsync();

using var githubCopilotHttpClient = app.Services.GetRequiredService<GithubCopilotHttpClient>();

var chatCompletionsDto = new GithubCopilotHttpClient.ChatCompletionsDto
{
    FrequencyPenalty = 0,
    PresencePenalty = 0,
    Temperature = 0,
    TopP = 1,
    N = 1,
    Stream = false,
    Model = "gpt-4.1",
    Messages = new List<GithubCopilotHttpClient.Message>
    {
        new GithubCopilotHttpClient.Message
        {
            Role = "user",
            Content = "create request dto based on JSON"
        }
    }
};

var data = await githubCopilotHttpClient.GetCompletionAsync(chatCompletionsDto);

Console.ReadKey();