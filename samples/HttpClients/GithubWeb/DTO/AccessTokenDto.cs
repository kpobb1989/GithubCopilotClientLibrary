using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubWeb.DTO
{
    internal class AccessTokenDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = null!;

        [JsonPropertyName("token_type")]
        public string TokenType { get; init; } = null!;

        [JsonPropertyName("scope")]
        public string Scope { get; init; } = null!;
    }
}
