public class ApiResponse<T>
/// <summary>
/// 期待所有API回傳格式為
// {
//   "success": false,
//   "data": null,
//   "errorMessage": "Conversation not found",
//   "errorCode": 40401, 可以在Common/ErrorCode.cs裡面找到對應狀態
//   "errorDetail": "資料庫內找不到對話"
// }
/// </summary>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    // 🔽 可加上錯誤代碼與詳細訊息
    public int? ErrorCode { get; set; }
    public string? ErrorDetail { get; set; }

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };

    public static ApiResponse<T> Fail(string message, int? code = null, string? detail = null) =>
        new() { Success = false, ErrorMessage = message, ErrorCode = code, ErrorDetail = detail };
}
