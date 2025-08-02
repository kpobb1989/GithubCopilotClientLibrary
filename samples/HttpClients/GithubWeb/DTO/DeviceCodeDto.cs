using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubWeb.DTO
{
    internal class DeviceCodeDto
    {
        [JsonPropertyName("device_code")]
        public string DeviceCode { get; init; } = null!;

        [JsonPropertyName("interval")]
        public int Interval { get; init; }

        [JsonPropertyName("user_code")]
        public string UserCode { get; init; } = null!;

        [JsonPropertyName("verification_uri")]
        public string VerificationUri { get; init; } = null!;
    }
}
