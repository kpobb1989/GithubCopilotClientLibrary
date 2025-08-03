using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Usage
{
    internal class QuotaSnapshots
    {
        [JsonProperty("chat")]
        public QuotaDetail Chat { get; set; } = new();

        [JsonProperty("completions")]
        public QuotaDetail Completions { get; set; } = new();

        [JsonProperty("premium_interactions")]
        public QuotaDetail PremiumInteractions { get; set; } = new();
    }
}
