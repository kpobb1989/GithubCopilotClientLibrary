using GithubApiProxy.HttpClients.GithubApi.DTO;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Usage;

namespace GithubApiProxy.Abstractions.HttpClients
{
    public interface IGithubApiHttpClient : IDisposable
    {
        void SetAccessToken(string accessToken);

        Task<CopilotTokenDto> GetCopilotTokenAsync(CancellationToken ct = default);

        Task<CopilotUsageResponse> GetCopilonUsageAsync(CancellationToken ct = default);
    }
}
