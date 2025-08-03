using GithubApiProxy.Abstractions;
using GithubApiProxy.Abstractions.HttpClients;
using GithubApiProxy.HttpClients.GithubApi;
using GithubApiProxy.HttpClients.GithubCopilot;
using GithubApiProxy.HttpClients.GithubWeb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace GithubApiProxy
{
    internal static class GithubCopilotModule
    {
        public static void AddGithubCopilotModule(this IServiceCollection services, GithubCopilotOptions? options = null)
        {
            options ??= new GithubCopilotOptions();

            services.AddHttpClient(nameof(GithubWebHttpClient), client =>
            {
                client.BaseAddress = new Uri("https://github.com");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient(nameof(GithubApiHttpClient), client =>
            {
                client.BaseAddress = new Uri("https://api.github.com");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
                client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", options.GithubApiVersion);
            });

            services.AddHttpClient(nameof(GithubCopilotHttpClient), client =>
            {
                client.BaseAddress = new Uri("https://api.individual.githubcopilot.com");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Editor-Version", options.EditorVersion);
                client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", options.GithubApiVersion);
            });

            services.AddSingleton<IGithubWebHttpClient, GithubWebHttpClient>();
            services.AddSingleton<IGithubApiHttpClient, GithubApiHttpClient>();
            services.AddSingleton<IGithubCopilotHttpClient, GithubCopilotHttpClient>();
            services.AddSingleton<IGithubCopilotClient>(sp =>
            {
                var githubApiHttpClient = sp.GetRequiredService<IGithubApiHttpClient>();
                var githubWebHttpClient = sp.GetRequiredService<IGithubWebHttpClient>();
                var githubCopilotHttpClient = sp.GetRequiredService<IGithubCopilotHttpClient>();
                var logger = sp.GetRequiredService<ILogger<GithubCopilotClient>>();
                var jsonSerializer = sp.GetRequiredService<JsonSerializer>();
                var opts = sp.GetRequiredService<GithubCopilotOptions>();

                return new GithubCopilotClient(githubApiHttpClient, githubWebHttpClient, githubCopilotHttpClient, logger, jsonSerializer, opts);
            });

            services.AddSingleton(options);

            services.AddLogging();

            services.AddSingleton(JsonSerializer.Create(new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Converters =
                [
                    new StringEnumConverter(new CamelCaseNamingStrategy())
                ]
            }));
        }
    }
}
