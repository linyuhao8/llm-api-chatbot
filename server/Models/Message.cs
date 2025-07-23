using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Chat.Models
{
    // 角色         說明                                     用途範例
    // User        使用者傳送的訊息（你在介面輸入的 prompt）	「請問你是誰？」
    // Assistant   AI 模型（像 GPT、Claude）生成的回應	       「我是 AI 助手，請問有什麼可以幫你？」
    // System      系統指令或角色設定，用來控制 AI 的行為與角色，不會顯示給使用者	「你是一位禮貌的翻譯機器人」


public enum RoleType
{
    [EnumMember(Value = "user")]
    User,

    [EnumMember(Value = "assistant")]
    Assistant,

    [EnumMember(Value = "system")]
    System
}
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ConversationId { get; set; }

        [ForeignKey("ConversationId")]
        //EF 會幫我處理這個關聯，請不要報 null 警告。
        //這表示：Message 一定會對應到一個 Conversation，但實際上這個屬性可能直到 EF 載入資料時才會被填入（例如 .Include() 才會有值）。
        public Conversation Conversation { get; set; } = null!;

        [Required]
        // [JsonConverter(typeof(JsonStringEnumConverter))] // 👉 這裡也加保險
        public RoleType Role { get; set; } = RoleType.User;

        [Required]
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
