using Chat.Dtos;
using Deepseek.Dtos;
public interface IChatService
{
    Task<object?> GetConversationWithMessages(int conversationId);
    Task<IEnumerable<object>> GetUserConversations(string userId);
    Task<IEnumerable<object>> GetAllConversations();
    Task<object> CreateConversation(string? title , string? userId);
    Task<MessageResponseDto?> AddMessage(int conversationId, CreateMessageDto messageDto);

    Task<DeepSeekResponseDto?> AskAndSaveAsync(int conversationId, List<MessageDto> messages, string provider);
}
