using GithubApiProxy.HttpClients.GithubWeb.DTO;

namespace GithubApiProxy.Abstractions.HttpClients
{
    public interface IGithubWebHttpClient
    {
        Task<DeviceCodeDto> GetDeviceCodeAsync(CancellationToken ct = default);

        Task<AccessTokenDto> GetAccessTokenAsync(string deviceCode, int interval, CancellationToken ct = default);
    }
}
