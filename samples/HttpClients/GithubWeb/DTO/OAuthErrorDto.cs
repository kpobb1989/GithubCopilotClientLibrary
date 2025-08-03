using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubWeb.DTO
{
    internal class OAuthErrorDto
    {
        [JsonProperty("error")]
        public string? Error { get; set; }

        [JsonProperty("error_description")]
        public string? ErrorDescription { get; set; }
    }
}
