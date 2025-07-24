<template>
  <div id="app">
    <div class="app-container">
      <!-- 側邊欄 -->
      <div class="sidebar" :class="{ 'mobile-hidden': !showSidebar }">
        <div class="sidebar-header">
          <!-- 下方btn -->
          <button class="new-chat-btn" @click="createNewChat">
            <span>+</span>
            新對話
          </button>
        </div>

        <div class="chat-history">
          <div v-if="loadingChats" class="loading-indicator">載入中...</div>

          <button
            v-for="chat in chatHistory"
            :key="chat.id"
            :class="['chat-item', { active: chat.id === currentChatId }]"
          >
            <!-- 上方btn @click="loadChat(chat.id)" -->
            <div class="chat-item-content">
              <div class="chat-item-title">{{ chat.title }}</div>
              <!-- <div class="chat-item-preview">{{ chat.preview }}</div> -->
            </div>
            <div class="chat-item-actions">
              <!-- 下方btn @click.stop="deleteChat(chat.id)" -->
              <button class="delete-btn">🗑️</button>
            </div>
          </button>

          <div
            v-if="chatHistory.length === 0 && !loadingChats"
            class="loading-indicator"
          >
            尚無對話記錄
          </div>
        </div>
      </div>

      <!-- 遮罩層 -->
      <div
        class="mobile-overlay"
        :class="{ show: showSidebar }"
        @click="toggleSidebar"
      ></div>

      <!-- 主要內容區域 -->
      <div class="main-content">
        <div class="chat-header">
          <button class="mobile-menu-btn" @click="toggleSidebar">☰</button>
          <div>
            <h1>{{ currentChatTitle }}</h1>
            <p>AI 助手</p>
          </div>
        </div>

        <div class="chat-messages" ref="messagesContainer">
          <div v-if="messages.length === 0" class="empty-state">
            <h2>你好！我是 ChatGPT</h2>
            <p>
              我是一個 AI
              助手，可以回答問題、協助寫作、解決問題等。請問我能為您做些什麼？
            </p>
            <!-- <div class="suggestion-grid">
              <div
                class="suggestion-card"
                @click="sendSuggestion('解釋量子物理的基本概念')"
              >
                <h3>🔬 解釋概念</h3>
                <p>幫我解釋複雜的科學概念</p>
              </div>
              <div
                class="suggestion-card"
                @click="sendSuggestion('寫一封正式的商業郵件')"
              >
                <h3>✍️ 協助寫作</h3>
                <p>幫我撰寫各種文件和內容</p>
              </div>
              <div
                class="suggestion-card"
                @click="sendSuggestion('如何提高工作效率？')"
              >
                <h3>💡 提供建議</h3>
                <p>給我實用的建議和技巧</p>
              </div>
              <div
                class="suggestion-card"
                @click="sendSuggestion('創建一個學習計劃')"
              >
                <h3>📚 規劃學習</h3>
                <p>幫我制定學習和成長計劃</p>
              </div>
            </div> -->
          </div>

          <div
            v-for="message in messages"
            :key="message.id"
            :class="['message', message.type]"
          >
            <div class="message-wrapper">
              <div class="message-avatar">
                {{ message.type === "user" ? "U" : "AI" }}
              </div>
              <div class="message-content">
                <div v-if="message.type === 'assistant' && message.isTyping">
                  {{ message.displayText
                  }}<span
                    v-if="message.displayText !== message.text"
                    class="typing-cursor"
                  ></span>
                </div>
                <div v-else>{{ message.text }}</div>
              </div>
            </div>
          </div>

          <div v-if="isTyping" class="message assistant">
            <div class="message-wrapper">
              <div class="message-avatar">AI</div>
              <div class="message-content">
                <div class="typing-indicator">
                  <div class="typing-dot"></div>
                  <div class="typing-dot"></div>
                  <div class="typing-dot"></div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="chat-input">
          <div class="input-container">
            <div class="input-wrapper">
              <!-- @keydown="handleKeyDown" -->
              <textarea
                v-model="newMessage"
                @input="autoResize"
                placeholder="輸入訊息..."
                class="message-input"
                ref="messageInput"
                :disabled="isTyping"
              ></textarea>
              <!-- @click="sendMessage" -->
              <button
                :disabled="!newMessage.trim() || isTyping"
                class="send-button"
              >
                ↑
              </button>
            </div>
            <div class="controls">
              <div class="char-count">{{ newMessage.length }}/2000</div>
              <!-- //@click="clearMessages" -->
              <button class="clear-button">清除對話</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import "~/assets/chatbot.css";
import { ref, reactive, onMounted, nextTick } from "vue";
import type { ChatResponse, ChatSummary } from "~/types/chat";
import type { ApiResponse } from "~/types/api-response";
import type { Conversation } from "~/types/conversation";
import { useRoute } from "vue-router";
const route = useRoute();

const fakeChatRes: ChatResponse = {
  success: true,
  data: {
    id: "531f26a7-7292-4629-a5cc-40b5d4998147",
    object: "chat.completion",
    created: 1753071963,
    model: "deepseek-chat",
    choices: [
      {
        index: 0,
        message: {
          role: "assistant",
          content:
            "你好呀！😊 很高兴见到你～今天有什么想聊的或者需要帮忙的吗？无论是闲聊、问题解答还是随便聊聊日常，我都在这里哦！✨",
        },
        logprobs: null,
        finish_reason: "stop",
      },
    ],
    usage: {
      prompt_tokens: 5,
      completion_tokens: 36,
      total_tokens: 41,
      prompt_tokens_details: {
        cached_tokens: 0,
      },
      prompt_cache_hit_tokens: 0,
      prompt_cache_miss_tokens: 5,
    },
    system_fingerprint: "fp_8802369eaa_prod0623_fp8_kvcache",
  },
  errorMessage: null,
};

// --- 模擬資料庫 ---
class MockDatabase {
  chats: any[];
  nextChatId: number;
  nextMessageId: number;

  constructor() {
    this.chats = [
      {
        id: 1,
        title: "量子物理基礎",
        preview: "解釋量子物理的基本概念...",
        createdAt: new Date(2024, 0, 15),
        messages: [
          {
            id: 1,
            type: "user",
            text: "解釋量子物理的基本概念",
            timestamp: "14:30",
          },
          {
            id: 2,
            type: "assistant",
            text: "量子物理是現代物理學的重要分支...",
            timestamp: "14:31",
          },
        ],
      },
      // 其餘略...
    ];
    this.nextChatId = 5;
    this.nextMessageId = 9;
  }

  async delay(ms = 300) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }

  async getChats() {
    await this.delay();
    return [...this.chats]
      .sort((a, b) => b.createdAt.getTime() - a.createdAt.getTime())
      .reverse();
  }

  async getChat(chatId: number) {
    await this.delay();
    return this.chats.find((chat) => chat.id === chatId);
  }

  async createChat(title = "新對話") {
    await this.delay();
    const newChat = {
      id: this.nextChatId++,
      title,
      preview: "",
      createdAt: new Date(),
      messages: [],
    };
    this.chats.push(newChat);
    return newChat;
  }

  async saveMessage(chatId: number, message: any) {
    await this.delay(100);
    const chat = this.chats.find((c) => c.id === chatId);
    if (chat) {
      message.id = this.nextMessageId++;
      chat.messages.push(message);

      if (message.type === "user" && !chat.preview) {
        chat.preview = message.text.substring(0, 30) + "...";
      }

      if (chat.title === "新對話" && message.type === "user") {
        chat.title =
          message.text.substring(0, 20) +
          (message.text.length > 20 ? "..." : "");
      }
    }
    return message;
  }

  async deleteChat(chatId: number) {
    await this.delay();
    const index = this.chats.findIndex((chat) => chat.id === chatId);
    if (index > -1) {
      this.chats.splice(index, 1);
      return true;
    }
    return false;
  }
}

// --- 狀態與參照變數 ---
const db = new MockDatabase();

const messages = ref<any[]>([]);
const newMessage = ref("");
const isTyping = ref(false);
const chatHistory = ref<Conversation[]>([]);
const currentChatId = ref<number | null>(null);
const currentChatTitle = ref("ChatGPT");
const loadingChats = ref(false);
const showSidebar = ref(false);
const userId = ref<number>(1);
const fetchKey = ref(Date.now());

const messageInput = ref<HTMLTextAreaElement | null>(null);
const messagesContainer = ref<HTMLDivElement | null>(null);

// --- 生命周期 ---
onMounted(async () => {
  console.log("onMounted 觸發");
  if (window.innerWidth > 768) showSidebar.value = true;
  await loadChatHistory();
  console.log("loadChatHistory 執行完");
  messageInput.value?.focus();
});
// --- 方法定義 ---
async function loadChatHistory() {
  if (!userId.value) return;

  loadingChats.value = true;

  try {
    const res = await fetch(
      `http://localhost:5208/api/Chat/users/${userId.value}/conversations`
    );
    const json = await res.json();
    if (json.success) {
      chatHistory.value = json.data ?? [];
    } else {
      console.warn("API 回傳錯誤:", json.errorMessage);
    }
  } catch (err) {
    console.error("網路或伺服器錯誤:", err);
  } finally {
    loadingChats.value = false;
  }
}

// async function loadChat(chatId: number) {
//   try {
//     const chat = await db.getChat(chatId);
//     if (chat) {
//       currentChatId.value = chatId;
//       currentChatTitle.value = chat.title;
//       messages.value = [...chat.messages];

//       if (window.innerWidth <= 768) showSidebar.value = false;

//       scrollToBottom();
//     }
//   } catch (err) {
//     console.error("載入聊天失敗:", err);
//   }
// }

async function createNewChat() {
  try {
    const res = await fetch("http://localhost:5208/api/Chat/conversations", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        accept: "*/*",
      },
      body: JSON.stringify({
        userId: "1",
      }),
    });

    const json: ApiResponse<Conversation> = await res.json();

    if (!json.success) {
      throw new Error(
        `HTTP 錯誤碼: ${res.status}, ${json.errorMessage ?? "API 回傳失敗"}`
      );
    }

    // ✅ 更新 UI 與狀態
    await loadChatHistory();

    currentChatId.value = json.data!.id;
    currentChatTitle.value = json.data!.title;
    messages.value = [];

    if (window.innerWidth <= 768) showSidebar.value = false;
    messageInput.value?.focus();
  } catch (err) {
    console.error("❌ 創建新聊天失敗:", err);
    // ❗️你也可以加上 toast 或 alert
    // toast.error("創建新聊天失敗，請稍後再試");
  }
}

// async function deleteChat(chatId: number) {
//   if (!confirm("確定要刪除這個對話嗎？")) return;
//   try {
//     await db.deleteChat(chatId);
//     chatHistory.value = chatHistory.value.filter((chat) => chat.id !== chatId);

//     if (currentChatId.value === chatId) {
//       messages.value = [];
//       currentChatId.value = null;
//       currentChatTitle.value = "ChatGPT";
//     }
//   } catch (err) {
//     console.error("刪除聊天失敗:", err);
//   }
// }

// async function sendMessage() {
//   if (!newMessage.value.trim() || isTyping.value) return;

//   if (!currentChatId.value) {
//     await createNewChat();
//   }

//   const userMessage = {
//     type: "user",
//     text: newMessage.value.trim(),
//     timestamp: formatTime(new Date()),
//     isTyping: false,
//     displayText: "",
//   };

//   const savedUserMessage = await db.saveMessage(
//     currentChatId.value!,
//     userMessage
//   );
//   messages.value.push(savedUserMessage);
//   newMessage.value = "";
//   scrollToBottom();

//   isTyping.value = true;

//   setTimeout(async () => {
//     const assistantMessage = {
//       type: "assistant",
//       text: generateMockResponse(savedUserMessage.text),
//       timestamp: formatTime(new Date()),
//     };
//     const savedAssistantMessage = await db.saveMessage(
//       currentChatId.value!,
//       assistantMessage
//     );
//     messages.value.push(savedAssistantMessage);
//     isTyping.value = false;
//     scrollToBottom();
//   }, 1000);
// }

// function sendSuggestion(text: string) {
//   newMessage.value = text;
//   sendMessage();
// }

// function clearMessages() {
//   if (!currentChatId.value) return;
//   if (!confirm("確定要清除這個對話內容嗎？")) return;

//   const chat = chatHistory.value.find(
//     (chat) => chat.id === currentChatId.value
//   );
//   if (chat) {
//     chat.messages = [];
//     messages.value = [];
//   }
// }

function autoResize(event: Event) {
  const textarea = event.target as HTMLTextAreaElement;
  textarea.style.height = "auto";
  textarea.style.height = textarea.scrollHeight + "px";
}

// function handleKeyDown(event: KeyboardEvent) {
//   if (event.key === "Enter" && !event.shiftKey) {
//     event.preventDefault();
//     sendMessage();
//   }
// }

function scrollToBottom() {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
    }
  });
}

function toggleSidebar() {
  showSidebar.value = !showSidebar.value;
}

function formatTime(date: Date) {
  const hours = date.getHours().toString().padStart(2, "0");
  const minutes = date.getMinutes().toString().padStart(2, "0");
  return `${hours}:${minutes}`;
}

// function generateMockResponse(userText: string) {
//   const replies = [
//     "這是一個很棒的問題，我們來深入探討一下...",
//     "根據您的描述，我建議可以這樣做...",
//     "讓我來幫您整理一下重點。",
//     "這個問題其實牽涉到幾個關鍵概念，我來說明一下。",
//     "我明白您的想法，以下是一些建議：",
//   ];
//   const randomIndex = Math.floor(Math.random() * replies.length);
//   return replies[randomIndex];
// }
</script>
