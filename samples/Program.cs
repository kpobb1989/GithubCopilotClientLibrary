using GithubApiProxy;
using GithubApiProxy.HttpClients.GithubApi;
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

// Register services
builder.Services.AddScoped<GithubWebHttpClient>();
builder.Services.AddScoped<GithubApiHttpClient>();

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

var userResponse = await githubApiHttpClient.GetUserAsync(); 
var copilotToken = await githubApiHttpClient.GetCopilotTokenAsync();

Console.ReadKey();