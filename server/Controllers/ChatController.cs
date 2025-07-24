using Microsoft.AspNetCore.Mvc;
using Chat.Dtos;
using Api.Controllers;
using Deepseek.Dtos;
using Api.Common;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ApiBaseController
{
    private readonly IChatService _chat;

    public ChatController(IChatService chat)
    {
        _chat = chat;
    }

    [HttpGet("conversations/{id}/messages")]
    public async Task<IActionResult> GetMessages(int id)
    {
        var result = await _chat.GetConversationWithMessages(id);
        return result == null
     ? Fail("找不到對話", 40401, $"Conversation ID: {id}")
     : Success(result);
    }

    [HttpGet("users/{userId}/conversations")]
    public async Task<IActionResult> GetUserConversations(string userId)
    {
        var result = await _chat.GetUserConversations(userId);
        return result == null ? Fail("User not found") : Success(result);
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetAllConversations()
    {
        var result = await _chat.GetAllConversations();
        return result == null ? Fail("Fail", ErrorCodes.ConversationNotFound, "找不到對話資料") : Success(result);
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto dto)
    {
        var result = await _chat.CreateConversation(dto.Title, dto.UserId);
        return result == null ? Fail("DBError", ErrorCodes.DatabaseError) : Success(result);
    }

    [HttpPost("conversations/{id}/messages")]
    public async Task<IActionResult> AddMessage(int id, [FromBody] CreateMessageDto messageDto)
    {
        var result = await _chat.AddMessage(id, messageDto);
        return result == null ? Fail("Conversation not found", ErrorCodes.ConversationNotFound, "資料庫內找不到對話") : Success(result);
    }

    [HttpPost("{conversationId}/ask")]
    public async Task<IActionResult> AskAndSaveAsync(int conversationId, [FromBody] List<MessageDto> messages, [FromQuery] string provider = "DeepSeek")
    {
        var result = await _chat.AskAndSaveAsync(conversationId, messages, provider);
        if (result == null)
            return Fail("AI 回應失敗或找不到使用者訊息");
        else
            return Success(result);
    }

}