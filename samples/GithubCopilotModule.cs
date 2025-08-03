using GithubApiProxy.Abstractions;
using GithubApiProxy.Abstractions.HttpClients;
using GithubApiProxy.HttpClients.GithubApi;
using GithubApiProxy.HttpClients.GithubCopilot;
using GithubApiProxy.HttpClients.GithubWeb;
using Microsoft.Extensions.DependencyInjection;

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
                client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2025-04-01");
            });

            // Register http clients
            services.AddSingleton<IGithubWebHttpClient, GithubWebHttpClient>();
            services.AddSingleton<IGithubApiHttpClient, GithubApiHttpClient>();
            services.AddSingleton<IGithubCopilotHttpClient, GithubCopilotHttpClient>();
            services.AddSingleton<IGithubCopilotClient, GithubCopilotClient>();
            services.AddSingleton(options);
        }
    }
}
