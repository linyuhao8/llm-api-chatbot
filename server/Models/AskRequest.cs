using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Chat.Dtos;

namespace Api.Models // 或 Api.Models、Common.Models
{
    public enum AIProvider
    {
        DeepSeek,
    }
    public class AskRequest
    {
        public int? ConversationId { get; set; } // optional
        // 新增一個模型選擇欄位
        [Required(ErrorMessage = "請選擇模型")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AIProvider Provider { get; set; }

        [Required(ErrorMessage = "請輸入提示內容")]
        public List<MessageDto> Messages { get; set; } = new();
    }
}
