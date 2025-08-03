using GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Models;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Streaming;
using System.Runtime.CompilerServices;

namespace GithubApiProxy.Abstractions.HttpClients
{
    internal interface IGithubCopilotHttpClient : IDisposable
    {
        Task<ChatCompletionResponse> GetChatCompletionAsync(ChatCompletionRequest body, CancellationToken ct = default);

        IAsyncEnumerable<ChatCompletionStreamingResponse> GetChatCompletionStreamingAsync(ChatCompletionRequest request, CancellationToken ct = default);

        Task<ModelsResponse> GetModelsAsync(CancellationToken ct = default);
    }
}
