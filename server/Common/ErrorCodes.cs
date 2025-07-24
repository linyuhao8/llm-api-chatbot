namespace Api.Common
{
    /// <summary>
    /// 錯誤代碼集中管理，方便各處共用。
    /// </summary>
    public static class ErrorCodes
    {
        // 資源找不到相關
        public const int ConversationNotFound = 40401;
        public const int UserNotFound = 40402;
        public const int MessageNotFound = 40403;

        // 驗證或授權相關
        public const int Unauthorized = 40100;
        public const int Forbidden = 40300;
        public const int ValidationFailed = 40001;

        // 伺服器內部錯誤
        public const int AiServiceError = 50001;
        public const int AiServiceParseError = 50002;
        public const int ExternalApiError = 50003;
        public const int DatabaseError = 50004;

        // 請求格式或內容錯誤
        public const int BadRequest = 40000;
        public const int UnsupportedMediaType = 41500;

        // 使用者操作限制
        public const int RateLimitExceeded = 42900;
        public const int OperationNotAllowed = 40301;

        // 其他自訂錯誤
        public const int Conflict = 40900;
        public const int Timeout = 40800;
    }
}
