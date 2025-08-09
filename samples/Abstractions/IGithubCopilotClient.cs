using System;
using System.Threading.Tasks;
using GithubApiProxy.DTO;

namespace GithubApiProxy.Abstractions
{
    public interface IGithubCopilotClient : IDisposable
    {
        IEnumerable<GithubCopilotMessage> GetConversationHistory();
        Task<string?> GetTextCompletionAsync(string prompt, CancellationToken ct = default);
        Task<string?> GetTextCompletionAsync<TParams>(string prompt, string toolDescription, Func<TParams, Task<object?>> toolHandler, CancellationToken ct = default) where TParams : class;
        Task<T?> GetJsonCompletionAsync<T>(string prompt, CancellationToken ct = default) where T : class;
        IAsyncEnumerable<string?> GetChatCompletionAsync(string prompt, CancellationToken ct = default);
        Task<GithubCopiotUsage> GetCopilotUsageAsync(CancellationToken ct = default);

    }
}
