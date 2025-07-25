namespace Chat.Dtos
{
    public class CreateConversationDto
    {
        public string? Title { get; set; }
        public string? UserId { get; set; }
    }
    public class ConversationResponseDto
    {
        /// <summary>
        /// 對話 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 對話標題
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 最後一條訊息的內容，作為預覽用
        /// </summary>
        public string? Preview { get; set; }
    }
}
