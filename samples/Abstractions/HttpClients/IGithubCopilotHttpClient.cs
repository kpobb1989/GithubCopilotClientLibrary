using GithubApiProxy.HttpClients.GithubCopilot;

namespace GithubApiProxy.Abstractions.HttpClients
{
    public interface IGithubCopilotHttpClient : IDisposable
    {
        Task<ChatCompletionResponse> GetCompletionAsync(ChatCompletionsDto body, CancellationToken ct = default);
    }
}
