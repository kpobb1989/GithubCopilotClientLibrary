using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Usage
{
    public class QuotaDetail
    {
        [JsonProperty("entitlement")]
        public int Entitlement { get; set; }

        [JsonProperty("overage_count")]
        public int OverageCount { get; set; }

        [JsonProperty("overage_permitted")]
        public bool OveragePermitted { get; set; }

        [JsonProperty("percent_remaining")]
        public double PercentRemaining { get; set; }

        [JsonProperty("quota_id")]
        public string QuotaId { get; set; } = string.Empty;

        [JsonProperty("quota_remaining")]
        public double QuotaRemaining { get; set; }

        [JsonProperty("remaining")] 
        public int Remaining { get; set; }

        [JsonProperty("unlimited")]
        public bool Unlimited { get; set; }

        [JsonProperty("timestamp_utc")]
        public string TimestampUtc { get; set; } = string.Empty;
    }
}
