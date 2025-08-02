using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubApi.DTO
{
    internal class CopilotTokenDto
    {
        [JsonPropertyName("token")]
        public string Token { get; init; } = null!;

        [JsonPropertyName("refresh_in")]
        public long RefreshIn { get; init; }

        [JsonPropertyName("expires_at")]
        public long ExpiresAt { get; init; }
    }
}
