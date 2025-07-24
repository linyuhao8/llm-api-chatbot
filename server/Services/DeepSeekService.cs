using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
//Deepseek回應格式
using Deepseek.Dtos;
using Api.Models;
using Ai.Service;
using Chat.Dtos;
using System.Text.Json.Serialization;
using Api.Common;


//建立一個名為 DeepSeekService 的類別，透過 HTTP 請求去呼叫 DeepSeek 的 Chat Completion API（類似 OpenAI ChatGPT API），並回傳模型的回答。

/// <summary>
/// DeepSeek AI API 服務類別
/// 負責與 DeepSeek API 進行通訊並處理回應
/// </summary>
public class DeepSeekService : IAIService
{
    #region 私有欄位

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    // 常數定義 - 將固定值抽出為常數，方便維護
    private const string ApiUrl = "https://api.deepseek.com/v1/chat/completions";
    private const string DefaultModel = "deepseek-chat";
    private const double DefaultTemperature = 0.7;
    public string Provider => "DeepSeek";

    #endregion

    #region 建構子

    /// <summary>
    /// DeepSeekService 建構子
    /// 使用依賴注入模式接收 HttpClient 和 IConfiguration
    /// </summary>
    /// <param name="httpClient">HTTP 客戶端，用於發送 API 請求</param>
    /// <param name="configuration">設定檔存取介面，用於讀取 API 金鑰</param>
    public DeepSeekService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    #endregion

    #region 公開方法

    /// <summary>
    /// 向 DeepSeek API 發送問題並取得回應
    /// </summary>
    /// <param name="request">包含問題內容的請求物件</param>
    /// <returns>包含 API 回應的結果物件</returns>
    public async Task<ApiResponse<DeepSeekResponseDto>> AskAsync(List<MessageDto> messages)
    {

        try
        {
            // 設定授權標頭
            SetAuthorizationHeader();

            // 建立請求內容
            var content = CreateRequestContent(messages);

            // 發送 API 請求
            var response = await _httpClient.PostAsync(ApiUrl, content);

            // 處理回應
            return await ProcessResponseAsync(response);
        }
        catch (HttpRequestException ex)
        {
            return CreateErrorResponse($"網路請求失敗: {ex.Message}");
        }
        catch (Exception ex)
        {
            return CreateErrorResponse($"處理請求時發生錯誤: {ex.Message}");
        }
    }

    #endregion

    #region 私有輔助方法

    /// <summary>
    /// 設定 HTTP 請求的授權標頭
    /// </summary>
    private void SetAuthorizationHeader()
    {
        var apiKey = _configuration["DeepSeek:ApiKey"]
            ?? throw new InvalidOperationException("找不到 DeepSeek API 金鑰設定");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);
    }

    /// <summary>
    /// 建立 API 請求的 JSON 內容
    /// </summary>
    /// <param name="request">使用者請求物件</param>
    /// <returns>格式化的 HTTP 內容</returns>
    private static StringContent CreateRequestContent(List<MessageDto> messages)
    {
        var requestData = new
        {
            model = DefaultModel,
            messages = messages.Select(m => new { role = m.Role, content = m.Content }),
            temperature = DefaultTemperature
        };

        // ✅ 加上 enum 轉成小寫字串（符合 "user" 格式）
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        var json = JsonSerializer.Serialize(requestData, options);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }


    /// <summary>
    /// 處理 API 回應並轉換為標準格式
    /// </summary>
    /// <param name="response">HTTP 回應物件</param>
    /// <returns>標準化的 API 回應</returns>
    private static async Task<ApiResponse<DeepSeekResponseDto>> ProcessResponseAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // 處理錯誤回應
        if (!response.IsSuccessStatusCode)
        {
            return ParseErrorResponse(json, options);
        }

        // 處理成功回應
        return ParseSuccessResponse(json, options);
    }

    /// <summary>
    /// 解析錯誤回應
    /// </summary>
    /// <param name="json">錯誤回應的 JSON 字串</param>
    /// <param name="options">JSON 序列化選項</param>
    /// <returns>包含錯誤訊息的回應</returns>
    private static ApiResponse<DeepSeekResponseDto> ParseErrorResponse(string json, JsonSerializerOptions options)
    {
        try
        {
            var errorDto = JsonSerializer.Deserialize<DeepSeekErrorDto>(json, options);
            var message = errorDto?.Error?.Message ?? "API 回傳未知錯誤";

            return CreateErrorResponse(
                message,
                ErrorCodes.AiServiceError, // 錯誤代碼統一管理
                json // 原始錯誤內容
            );
        }
        catch
        {
            return CreateErrorResponse(
                $"無法解析錯誤回應: {json}",
                ErrorCodes.AiServiceParseError,
                json
            );
        }
    }

    /// <summary>
    /// 解析成功回應
    /// </summary>
    /// <param name="json">成功回應的 JSON 字串</param>
    /// <param name="options">JSON 序列化選項</param>
    /// <returns>包含資料的回應</returns>
    private static ApiResponse<DeepSeekResponseDto> ParseSuccessResponse(string json, JsonSerializerOptions options)
    {
        try
        {
            var result = JsonSerializer.Deserialize<DeepSeekResponseDto>(json, options);
            return new ApiResponse<DeepSeekResponseDto>
            {
                Success = true,
                Data = result
            };
        }
        catch (JsonException ex)
        {
            return CreateErrorResponse(
                "API 回傳格式錯誤",
                ErrorCodes.AiServiceParseError,
                ex.Message
            );
        }
    }


    /// <summary>
    /// 建立錯誤回應的輔助方法
    /// </summary>
    /// <param name="errorMessage">錯誤訊息</param>
    /// <returns>標準化的錯誤回應</returns>
    private static ApiResponse<DeepSeekResponseDto> CreateErrorResponse(string errorMessage, int? errorCode = null, string? detail = null)
    {
        return new ApiResponse<DeepSeekResponseDto>
        {
            Success = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
            ErrorDetail = detail
        };
    }

    #endregion
}