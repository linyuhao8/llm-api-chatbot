using Microsoft.AspNetCore.Mvc;
using Api.Models; // AskRequest 的命名空間（如果你自己定義）
using Ai.Service; // IAIService 與 AIServiceFactory 的命名空間

// 標記此類別是 API Controller，會自動處理 Model 驗證錯誤與 JSON 格式的輸入/輸出
[ApiController]


// 設定路由規則：api/deepseek/ai（[controller] 會取用類別名稱 AiController 的 "Ai"）
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    // 注入 DeepSeekService，用來與 DeepSeek API 溝通private readonly AIServiceFactory _factory;
    private readonly AIServiceFactory _factory;

    public AiController(AIServiceFactory factory)
    {
        _factory = factory;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskRequest request)
    {
        var service = _factory.GetService(request.Provider.ToString());
        var result = await service.AskAsync(request.Prompt);
        return Ok(result);
    }

}
