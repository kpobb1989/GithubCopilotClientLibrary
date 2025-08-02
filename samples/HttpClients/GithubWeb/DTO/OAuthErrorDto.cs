using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubWeb.DTO
{
    internal class OAuthErrorDto
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }
    }
}
