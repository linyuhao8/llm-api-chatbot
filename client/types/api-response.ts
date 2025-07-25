// 假設你在這個檔案裡定義，並且有從別處 import DeepSeekResponse 型別
import type { DeepSeekResponse } from "./DeepSeekRespones"; // <-- 這邊改成你實際路徑

export type ApiResponse<T> = {
  success: boolean;
  data: T | null;
  errorMessage?: string;
  errorCode?: number;
  errorDetail?: string;
};

export interface AskResultData {
  aiResponse: DeepSeekResponse;
  userMessageId: number;
  aiMessageId: number;
}
