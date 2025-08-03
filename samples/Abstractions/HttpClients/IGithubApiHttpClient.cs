using GithubApiProxy.HttpClients.GithubApi.DTO;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Usage;

namespace GithubApiProxy.Abstractions.HttpClients
{
    internal interface IGithubApiHttpClient : IDisposable
    {
        void SetAccessToken(string accessToken);

        Task<CopilotTokenResponse> GetCopilotTokenAsync(CancellationToken ct = default);

        Task<CopilotUsageResponse> GetCopilonUsageAsync(CancellationToken ct = default);
    }
}
