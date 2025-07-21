// IAIService.cs
using Api.Models;
using Deepseek.Dtos;

namespace Ai.Service
{
    public interface IAIService
    {
        string Provider { get; } // 每個實作都要定義是什麼 Provider，例如 "deepseek" 或 "openai"

        Task<ApiResponse<DeepSeekResponseDto>> AskAsync(string prompt);
    }
}
