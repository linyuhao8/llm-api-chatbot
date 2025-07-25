using Microsoft.AspNetCore.Mvc;
using Api.Models;

namespace Api.Controllers
{
    [ApiController]
    /// <summary>
    /// ApiBaseController 提供標準化 API 回應格式，包含成功與失敗回傳方法。
    /// 
    /// 使用範例：
    ///
    /// // 回傳成功資料
    /// return Success(dataObject);
    ///
    /// // 回傳失敗訊息（僅訊息）
    /// return Fail("找不到資源");
    ///
    /// // 回傳失敗訊息（含錯誤代碼與細節）
    /// return Fail("服務異常", 50001, "AI 服務未啟動");
    /// Or
    /// return Fail("找不到對話", ErrorCodes.ConversationNotFound ,在資料庫內未找到對話);
    /// Error Code 在 Common資料夾
    ///
    /// // 根據 ApiResponse<T> 自動判斷成功或失敗回應
    /// var response = await _service.DoSomethingAsync();
    /// return FromApiResponse(response);
    /// </summary>
    public class ApiBaseController : ControllerBase
    {
        /// <summary>
        /// 回傳成功結果
        /// </summary>
        protected IActionResult Success<T>(T data)
            => Ok(ApiResponse<T>.Ok(data));

        /// <summary>
        /// 回傳失敗（只有訊息）
        /// </summary>
        protected IActionResult Fail(string message)
            => Ok(ApiResponse<object>.Fail(message));

        /// <summary>
        /// 回傳失敗（含錯誤碼與詳細訊息）
        /// </summary>
        protected IActionResult Fail(string message, int? code = null, string? detail = null)
            => Ok(ApiResponse<object>.Fail(message, code, detail));

        /// <summary>
        /// 根據 ApiResponse 自動決定回應格式
        /// </summary>
        protected IActionResult FromApiResponse<T>(ApiResponse<T> response)
        {
            return Ok(response);
        }

    }


}
