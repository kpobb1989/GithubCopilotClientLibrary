using GithubApiProxy.HttpClients.GithubApi.DTO;

namespace GithubApiProxy.Abstractions.HttpClients
{
    public interface IGithubApiHttpClient : IDisposable
    {
        void SetAccessToken(string accessToken);

        Task<CopilotTokenDto> GetCopilotTokenAsync(CancellationToken ct = default);
    }
}
