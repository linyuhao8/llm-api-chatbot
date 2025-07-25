// composables/useChatApi.ts
import type { Conversation } from "~/types/conversation";
import type { AskResultData, ApiResponse } from "~/types/api-response";

/**
 * 取得使用者的對話清單
 */
export async function fetchChatHistory(
  userId: string
): Promise<ApiResponse<Conversation[]>> {
  const res = await fetch(
    `http://localhost:5208/api/Chat/users/${userId}/conversations`
  );
  return await res.json();
}

/**
 * 取得指定對話的訊息
 */
export async function fetchMessages(conversationId: number): Promise<
  ApiResponse<{
    id: number;
    title: string;
    messages: any[];
  }>
> {
  const res = await fetch(
    `http://localhost:5208/api/Chat/conversations/${conversationId}/messages`
  );
  return await res.json();
}

/**
 * 建立新的對話（會返回新 conversation 資訊）
 */
export async function createConversation(
  userId: string,
  title?: string
): Promise<ApiResponse<Conversation>> {
  const res = await fetch("http://localhost:5208/api/Chat/conversations", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      accept: "*/*",
    },
    body: JSON.stringify({ userId, title }),
  });

  return await res.json();
}

/**
 * 傳送訊息給 AI 並取得回覆
 */
export async function askAI(
  conversationId: number,
  userMessages: { role: string; content: string }[],
  provider: string = "DeepSeek"
): Promise<ApiResponse<AskResultData>> {
  const res = await fetch(
    `http://localhost:5208/api/Chat/${conversationId}/ask?provider=${provider}`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        accept: "*/*",
      },
      body: JSON.stringify(userMessages),
    }
  );

  return await res.json();
}
