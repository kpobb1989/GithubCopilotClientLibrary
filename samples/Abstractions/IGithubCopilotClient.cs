using GithubApiProxy.DTO;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Usage;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GithubApiProxy.Abstractions
{
    public interface IGithubCopilotClient : System.IDisposable
    {
        Task AuthenticateAsync(bool force = false, CancellationToken ct = default);
        Task<string?> GetTextCompletionAsync(string prompt, CancellationToken ct = default);
        Task<T?> GetJsonCompletionAsync<T>(string prompt, CancellationToken ct = default) where T : class;
        IAsyncEnumerable<Message?> GetChatCompletionAsync(string prompt, CancellationToken ct = default);
        Task<GithubCopiotUsage> GetCopilotUsageAsync(CancellationToken ct = default);
    }
}
