using GithubApiProxy.HttpClients.GithubCopilot;

namespace GithubApiProxy.Abstractions
{
    public interface IGithubCopilotClient : IDisposable
    {
        Task AuthenticateAsync(CancellationToken ct = default);

        Task<string?> GetTextCompletionAsync(string prompt, CancellationToken ct = default);
    }
}
