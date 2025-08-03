using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubWeb.DTO
{
    public class AccessTokenDto
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; init; } = null!;

        [JsonProperty("token_type")]
        public string TokenType { get; init; } = null!;

        [JsonProperty("scope")]
        public string Scope { get; init; } = null!;
    }
}
