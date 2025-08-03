using Newtonsoft.Json;
using System.Collections.Generic;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Models
{
    public class ModelsResponse
    {
        [JsonProperty("data")]
        public List<Model> Data { get; set; } = [];
    }

    public class Model
    {
        [JsonProperty("capabilities")]
        public ModelCapabilities? Capabilities { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("model_picker_enabled")]
        public bool ModelPickerEnabled { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("preview")]
        public bool? Preview { get; set; }

        [JsonProperty("vendor")]
        public string? Vendor { get; set; }

        [JsonProperty("version")]
        public string? Version { get; set; }

        [JsonProperty("policy")]
        public Policy? Policy { get; set; }
    }

    public class Policy
    {
        [JsonProperty("state")]
        public string? State { get; set; }

        [JsonProperty("terms")]
        public string? Terms { get; set; }
    }

    public class ModelCapabilities
    {
        [JsonProperty("family")]
        public string? Family { get; set; }

        [JsonProperty("limits")]
        public ModelLimits? Limits { get; set; }

        [JsonProperty("supports")]
        public ModelSupports? Supports { get; set; }

        [JsonProperty("tokenizer")]
        public string? Tokenizer { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }
    }

    public class ModelLimits
    {
        [JsonProperty("max_context_window_tokens")]
        public int? MaxContextWindowTokens { get; set; }

        [JsonProperty("max_output_tokens")]
        public int? MaxOutputTokens { get; set; }

        [JsonProperty("max_prompt_tokens")]
        public int? MaxPromptTokens { get; set; }

        [JsonProperty("vision")]
        public VisionLimits? Vision { get; set; }
    }

    public class VisionLimits
    {
        [JsonProperty("max_prompt_image_size")]
        public int? MaxPromptImageSize { get; set; }

        [JsonProperty("max_prompt_images")]
        public int? MaxPromptImages { get; set; }

        [JsonProperty("supported_media_types")]
        public List<string>? SupportedMediaTypes { get; set; }
    }

    public class ModelSupports
    {
        [JsonProperty("tool_calls")]
        public bool? ToolCalls { get; set; }

        [JsonProperty("parallel_tool_calls")]
        public bool? ParallelToolCalls { get; set; }

        [JsonProperty("structured_outputs")]
        public bool? StructuredOutputs { get; set; }

        [JsonProperty("streaming")]
        public bool? Streaming { get; set; }

        [JsonProperty("vision")]
        public bool? Vision { get; set; }
    }
}
