using GithubApiProxy.HttpClients.GithubCopilot.DTO;

namespace GithubApiProxy.Abstractions.HttpClients
{
    public interface IGithubCopilotHttpClient : IDisposable
    {
        Task<ChatCompletionResponse> GetChatCompletionAsync(ChatCompletionRequest body, CancellationToken ct = default);
    }
}
