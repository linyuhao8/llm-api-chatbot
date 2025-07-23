using Microsoft.AspNetCore.Mvc;
using Api.Models; // 這是你 ApiResponse<T> 的命名空間

/// <summary>
/// ApiBaseController 提供統一的 API 回傳格式，所有 API Controller 可繼承此類別，使用 Success / Fail 方法回傳標準格式。
/// </summary>
/// <remarks>
/// 標準回傳格式：
/// {
///     "success": true,
///     "data": {...},
///     "errorMessage": null
/// }
///
/// 範例用法：
/// public class ChatController : ApiBaseController
/// {
///     [HttpGet("conversations/{id}/messages")]
///     public async Task<IActionResult> GetMessages(int id)
///     {
///         var result = await _chatService.GetConversationWithMessages(id);
///         return result == null
///             ? Fail("Conversation not found")
///             : Success(result);
///     }
/// }
/// </remarks>


namespace Api.Controllers
{
    public class ApiBaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data) => Ok(ApiResponse<T>.Ok(data));
        protected IActionResult Fail(string message) => Ok(ApiResponse<object>.Fail(message));
    }
}