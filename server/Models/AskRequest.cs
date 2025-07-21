using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api.Models // 或 Api.Models、Common.Models
{
    public enum AIProvider
    {
        DeepSeek,
    }
    public class AskRequest
    {
        [Required(ErrorMessage = "請輸入提示內容")]
        public string Prompt { get; set; } = string.Empty;

        // 新增一個模型選擇欄位
        [Required(ErrorMessage = "請選擇模型")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AIProvider Provider { get; set; }
    }
}
