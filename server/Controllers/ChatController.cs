using Microsoft.AspNetCore.Mvc;
using Chat.Dtos;
using Api.Controllers;

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
        return result == null ? Fail("Conversation not found") : Success(result);
    }

    [HttpGet("users/{userId}/conversations")]
    public async Task<IActionResult> GetUserConversations(string userId)
    {
        var result = await _chat.GetUserConversations(userId);
        return Ok(result);
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetAllConversations()
    {
        var result = await _chat.GetAllConversations();
        return Ok(result);
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto dto)
    {
        var result = await _chat.CreateConversation(dto.Title, dto.UserId);
        return Ok(result);
    }

    [HttpPost("conversations/{id}/messages")]
    public async Task<IActionResult> AddMessage(int id, [FromBody] CreateMessageDto messageDto)
    {
        var result = await _chat.AddMessage(id, messageDto);
        return result == null ? NotFound("Conversation not found") : Ok(result);
    }

    [HttpPost("{conversationId}/ask")]
    public async Task<IActionResult> AskAndSaveAsync(int conversationId, [FromBody] List<MessageDto> messages, [FromQuery] string provider = "DeepSeek")
    {
        var result = await _chat.AskAndSaveAsync(conversationId, messages, provider);
        return result == null ? NotFound("AI 回應失敗或訊息格式錯誤") : Ok(result);
    }

}
