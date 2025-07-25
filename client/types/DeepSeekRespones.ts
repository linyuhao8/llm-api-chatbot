// DeepSeek AI 回應型別
export interface DeepSeekResponse {
  id: string;
  object: string;
  created: number;
  model: string;
  choices: DeepSeekChoice[];
  usage?: DeepSeekUsage;
  system_fingerprint?: string;
}

export interface DeepSeekChoice {
  index: number;
  message: DeepSeekMessage;
  logprobs: any | null;
  finish_reason: string | null;
}

export interface DeepSeekMessage {
  role: string;
  content: string;
}

export interface DeepSeekUsage {
  prompt_tokens: number;
  completion_tokens: number;
  total_tokens: number;
  prompt_tokens_details?: DeepSeekPromptTokensDetails;
  prompt_cache_hit_tokens?: number;
  prompt_cache_miss_tokens?: number;
}

export interface DeepSeekPromptTokensDetails {
  cached_tokens?: number;
}
