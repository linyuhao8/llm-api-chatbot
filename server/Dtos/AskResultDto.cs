using Deepseek.Dtos;
namespace Chat.Dtos;

public class AskResultDto
{
    /// <summary>
    /// AI 服務回傳的完整結果
    /// </summary>
    public DeepSeekResponseDto AiResponse { get; set; } = default!;

    /// <summary>
    /// 資料庫儲存的使用者訊息 ID
    /// </summary>
    public int UserMessageId { get; set; }

    /// <summary>
    /// 資料庫儲存的 AI 回覆訊息 ID (如果有)
    /// </summary>
    public int? AiMessageId { get; set; }
}

