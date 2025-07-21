using System.Text.Json.Serialization;

namespace Deepseek.Dtos
{
    // 成功回應 DTO
    public class DeepSeekResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;  // 回傳的唯一識別 ID，重要且一定要有

        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;  // 通常是物件類型描述，如 "chat.completion"

        [JsonPropertyName("created")]
        public long Created { get; set; }  // 建立時間戳（Unix時間），通常會有

        [JsonPropertyName("model")]
        public string Model { get; set; } = null!;  // 使用的模型名稱

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; } = null!;  // 回答選項，必須有且至少一筆

        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }  // 使用情況資訊，可選

        [JsonPropertyName("system_fingerprint")]
        public string? SystemFingerprint { get; set; }  // 系統指紋，非必要
    }

    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }  // 選項索引，通常必有

        [JsonPropertyName("message")]
        public Message Message { get; set; } = null!;  // 內容訊息，必有

        [JsonPropertyName("logprobs")]
        public object? Logprobs { get; set; }  // 日誌概率，通常可空

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }  // 完成原因，可空
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = null!;  // 角色，通常是 "assistant" 或 "user"，必有

        [JsonPropertyName("content")]
        public string Content { get; set; } = null!;  // 文字內容，必有
    }

    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }  // 輸入提示 tokens 數

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }  // 輸出完成 tokens 數

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }  // 總 tokens 數

        [JsonPropertyName("prompt_tokens_details")]
        public PromptTokensDetails? PromptTokensDetails { get; set; }  // 提示 tokens 詳細，可空

        [JsonPropertyName("prompt_cache_hit_tokens")]
        public int? PromptCacheHitTokens { get; set; }  // 提示快取命中數，可空

        [JsonPropertyName("prompt_cache_miss_tokens")]
        public int? PromptCacheMissTokens { get; set; }  // 提示快取未命中數，可空
    }

    public class PromptTokensDetails
    {
        [JsonPropertyName("cached_tokens")]
        public int? CachedTokens { get; set; }  // 快取的 tokens 數量，可空
    }

}
