using System.Text.Json.Serialization;
using Chat.Models;
namespace Chat.Dtos;

public class MessageDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoleType Role { get; set; } = RoleType.User; // user / assistant / system
    public string Content { get; set; } = string.Empty;
}
public class CreateMessageDto : MessageDto { }
public class UpdateMessageDto : MessageDto { }


public class MessageResponseDto
{
    public int Id { get; set; }
    public RoleType Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
