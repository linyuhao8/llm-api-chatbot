using Ai.Service;

/// <summary>
/// AI 服務工廠，用來依據指定的 Provider 名稱取得對應的 AI Service 實作。
/// 支援注入多個 IAIService 實作（例如 DeepSeekService、OpenAIService 等）
/// </summary>
public class AIServiceFactory
{
    // 所有實作了 IAIService 的服務會透過 DI 容器注入進來
    private readonly IEnumerable<IAIService> _services;

    /// <summary>
    /// 建構子：由 DI 自動注入所有 IAIService 的實作
    /// </summary>
    /// <param name="services">所有註冊的 AI 服務實作</param>
    public AIServiceFactory(IEnumerable<IAIService> services)
    {
        _services = services;
    }

    /// <summary>
    /// 根據 provider 名稱取得對應的 AI 服務實作
    /// </summary>
    /// <param name="provider">提供者名稱（例如 "DeepSeek", "OpenAI"）</param>
    /// <returns>符合的 IAIService 實作</returns>
    /// <exception cref="Exception">若無對應實作，則拋出錯誤</exception>
    public IAIService GetService(string provider)
    {
        return _services.FirstOrDefault(s => s.Provider == provider)
            ?? throw new Exception($"AI Provider '{provider}' not found");
    }
}
