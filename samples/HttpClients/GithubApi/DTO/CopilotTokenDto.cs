using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubApi.DTO
{
    public class CopilotTokenDto
    {
        [JsonProperty("token")]
        public string Token { get; init; } = null!;

        [JsonProperty("refresh_in")]
        public long RefreshIn { get; init; }

        [JsonProperty("expires_at")]
        public long ExpiresAt { get; init; }
    }
}
