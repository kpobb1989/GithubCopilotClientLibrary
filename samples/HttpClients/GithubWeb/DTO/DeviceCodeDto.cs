using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubWeb.DTO
{
    public class DeviceCodeDto
    {
        [JsonProperty("device_code")]
        public string DeviceCode { get; init; } = null!;

        [JsonProperty("interval")]
        public int Interval { get; init; }

        [JsonProperty("user_code")]
        public string UserCode { get; init; } = null!;

        [JsonProperty("verification_uri")]
        public string VerificationUri { get; init; } = null!;
    }
}
