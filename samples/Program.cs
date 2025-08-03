using GithubApiProxy;
using GithubApiProxy.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Properly display special characters like emojis
Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddGithubCopilotModule();

var app = builder.Build();

var githubCopilotService = app.Services.GetRequiredService<IGithubCopilotClient>();

Console.WriteLine("Enter a prompt for GitHub Copilot:");

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


//class Response
//{
//    public List<DotNetCoreVersion>? Data { get; set; } // You always have to create a wrapper class for the response if the data is an array
//}

//class DotNetCoreVersion
//{
//    [Description("Version number in the format of Major.Minor.Patch")]
//    public string? Version { get; set; }

//    [Description("Release date of the version")]
//    public DateTime? ReleaseDate { get; set; }

//    [Description("End of life date of the version")]
//    public DateTime? EndOfLife { get; set; }
//}

//var a = await githubCopilotService.GetCopilotUsageAsync();

//Console.ReadKey();