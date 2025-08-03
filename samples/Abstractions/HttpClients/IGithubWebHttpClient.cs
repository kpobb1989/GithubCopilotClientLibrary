using GithubApiProxy.HttpClients.GithubWeb.DTO;

namespace GithubApiProxy.Abstractions.HttpClients
{
    internal interface IGithubWebHttpClient
    {
        Task<DeviceCodeResponse> GetDeviceCodeAsync(CancellationToken ct = default);

        Task<AccessTokenResponse> WaitForAccessTokenAsync(string deviceCode, int interval, CancellationToken ct = default);
    }
}
