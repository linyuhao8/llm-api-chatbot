using Chat.Dtos;
using App.Data;
using Microsoft.EntityFrameworkCore;
using Chat.Models;
using Api.Models;
using DbMessage = Chat.Models.Message;
using Deepseek.Dtos;
using DeepseekMessage = Deepseek.Dtos.Message;

public class ChatService : IChatService
{
    private readonly AppDbContext _db;
    private readonly AIServiceFactory _factory;

    public ChatService(AppDbContext db, AIServiceFactory factory)
    {
        _db = db;
        _factory = factory;
    }

    public async Task<object?> GetConversationWithMessages(int conversationId)
    {
        var conversation = await _db.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null) return null;

        return new
        {
            conversation.Id,
            conversation.Title,
            Messages = conversation.Messages
                .OrderBy(m => m.CreatedAt)
                .Select(m => new { m.Role, m.Content, m.CreatedAt })
                .ToList()
        };
    }

    public async Task<IEnumerable<object>> GetUserConversations(string userId)
    {
        var conversations = await _db.Conversations
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return conversations.Select(c => new { c.Id, c.Title, c.CreatedAt });
    }

    public async Task<IEnumerable<object>> GetAllConversations()
    {
        var conversations = await _db.Conversations
            .Include(c => c.Messages)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return conversations.Select(c => new
        {
            c.Id,
            c.Title,
            c.CreatedAt,
            Preview = c.Messages
            .OrderByDescending(m => m.CreatedAt)  // 最新訊息
            .Select(m => m.Content)
            .FirstOrDefault()
        });
    }

public async Task<object> CreateConversation(string? title, string? userId)
{
    var conversation = new Conversation
    {
        Title = title ?? "新對話",
        UserId = userId
    };

    _db.Conversations.Add(conversation);
    await _db.SaveChangesAsync();

    return new { conversation.Id, conversation.Title, conversation.CreatedAt };
}

    public async Task<MessageResponseDto?> AddMessage(int conversationId, CreateMessageDto messageDto)
    {
        var conversation = await _db.Conversations.FindAsync(conversationId);
        if (conversation == null) return null;

        var message = new DbMessage
        {
            ConversationId = conversationId,
            Role = messageDto.Role,
            Content = messageDto.Content,
        };
        _db.Messages.Add(message);
        await _db.SaveChangesAsync();

        return new MessageResponseDto
        {
            Id = message.Id,
            Role = message.Role,
            Content = message.Content,
            CreatedAt = message.CreatedAt
        };
    }


    public async Task<DeepSeekResponseDto?> AskAndSaveAsync(int conversationId, List<MessageDto> messages, string provider)
    {
        // 取得使用者最新訊息
        var lastUserMessage = messages.LastOrDefault(m => m.Role == RoleType.User);
        if (lastUserMessage == null) return null;

        var aiService = _factory.GetService(provider);
        var aiResponse = await aiService.AskAsync(messages);

        if (!aiResponse.Success || aiResponse.Data == null) return null;

        // 儲存使用者訊息
        await AddMessage(conversationId, new CreateMessageDto
        {
            Role = RoleType.User,
            Content = lastUserMessage.Content
        });

        // 儲存 AI 回覆訊息
        var aiContent = aiResponse.Data.Choices.FirstOrDefault()?.Message.Content;
        if (!string.IsNullOrWhiteSpace(aiContent))
        {
            await AddMessage(conversationId, new CreateMessageDto
            {
                Role = RoleType.Assistant,
                Content = aiContent
            });
        }

        return aiResponse.Data;
    }



}
