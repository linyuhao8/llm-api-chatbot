export interface ChatResponse {
  success: boolean;
  data: ChatData;
  errorMessage: string | null;
}

export interface ChatData {
  id: string;
  object: string;
  created: number;
  model: string;
  choices: ChatChoice[];
  usage: Usage | null;
  system_fingerprint: string | null;
}

export interface ChatChoice {
  index: number;
  message: ChatMessage;
  logprobs: any | null;
  finish_reason: string | null;
}

export interface ChatMessage {
  role: "user" | "assistant" | "system";
  content: string;
}

export interface Usage {
  prompt_tokens: number;
  completion_tokens: number;
  total_tokens: number;
  prompt_tokens_details?: {
    cached_tokens: number;
  };
  prompt_cache_hit_tokens?: number;
  prompt_cache_miss_tokens?: number;
}
