using GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Streaming;
using System.Runtime.CompilerServices;

namespace GithubApiProxy.Abstractions.HttpClients
{
    public interface IGithubCopilotHttpClient : IDisposable
    {
        Task<ChatCompletionResponse> GetChatCompletionAsync(ChatCompletionRequest body, CancellationToken ct = default);

        IAsyncEnumerable<ChatCompletionStreamingResponse> GetChatCompletionStreamingAsync(ChatCompletionRequest request, CancellationToken ct = default);
    }
}
