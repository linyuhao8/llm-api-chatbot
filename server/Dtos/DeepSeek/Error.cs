using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Deepseek.Dtos
{
    // 錯誤回應 DTO
    public class DeepSeekErrorDto
    {
        [JsonPropertyName("error")]
        public ErrorDetail Error { get; set; } = null!;  // 通常錯誤物件會存在，設為非 null
    }

    public class ErrorDetail
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;  // 錯誤訊息，一般必有

        [JsonPropertyName("type")]
        public string? Type { get; set; }  // 錯誤類型，可為空，可能沒提供

        [JsonPropertyName("param")]
        public string? Param { get; set; }  // 相關參數名，非必要，可能沒提供

        [JsonPropertyName("code")]
        public string? Code { get; set; }  // 錯誤代碼，可能沒提供，所以可空
    }

}
