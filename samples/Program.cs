using GithubApiProxy;
using GithubApiProxy.Abstractions;
using GithubApiProxy.DTO;
using GithubApiProxy.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

// Properly display special characters like emojis
Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

// Suppress System.Net.Http.HttpClient logs (set to Warning or higher)
builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);

builder.Services.AddGithubCopilotModule(new GithubCopilotOptions() { SystemPrompt = null });

var app = builder.Build();

var githubCopilotService = app.Services.GetRequiredService<IGithubCopilotClient>();

//while (true)
//{
//    var prompt = Console.ReadLine();

//    if (string.IsNullOrWhiteSpace(prompt))
//    {
//        Console.WriteLine("Prompt cannot be empty. Please enter a valid prompt.");
//        continue;
//    }

//    var text = await githubCopilotService.GetTextCompletionAsync(prompt);

//    Console.WriteLine(text);
//}

//var cts = new CancellationTokenSource();

//Console.WriteLine("CTRL+C to terminate the conversation.");
//#pragma warning disable CS4014
//Task.Run(async () =>
//{
//    while (true)
//    {
//        var keyInfo = Console.ReadKey(intercept: true);
//        if (keyInfo.Key == ConsoleKey.C && keyInfo.Modifiers == ConsoleModifiers.Control)
//        {
//            await cts.CancelAsync();
//        }
//    }
//});

//while (true)
//{
//    if (cts.Token.IsCancellationRequested)
//    {
//        cts = new CancellationTokenSource();
//    }

//    var prompt = Console.ReadLine()!;

//    try
//    {
//        await foreach (var chunk in githubCopilotService.GetChatCompletionAsync(prompt, ct: cts.Token))
//        {
//            await Task.Delay(50, cts.Token);

//            Console.Write(chunk?.Content);
//        }
//    }
//    catch (OperationCanceledException)
//    {
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Error: {ex.Message}");
//    }

//    Console.WriteLine();
//}

//var data = await githubCopilotService.GetJsonCompletionAsync<Response>("Return a list of all available .NET Core versions from the past five years.");

//Console.ReadKey();

var tools = new ToolBuilder().AddTool("Get the current weather for a given latitude and longitude.", async (GetWeatherRequest parameters) =>
{
    return new
    {
        temperature = 25,
        condition = "Sunny",
        lat = parameters.Lat,
        lon = parameters.Lng
    };
}).Build();

// Single tool usage
var resp = await githubCopilotService.GetTextCompletionAsync<GetWeatherRequest>(
    "What's the weather in Vinnytsia Latitude:49.2328, Longitude:28.48097?",
    tools
);
Console.WriteLine(resp);

Console.ReadKey();

public class GetWeatherRequest
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}