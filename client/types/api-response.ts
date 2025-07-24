// types/api-response.ts

export type ApiResponse<T> = {
  success: boolean;
  data: T | null;
  errorMessage?: string;
  errorCode?: number;
  errorDetail?: string;
};
