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
            @click="loadChat(chat.id)"
          >
            <div class="chat-item-content">
              <div class="chat-item-title">{{ chat.title }}</div>
              <div class="chat-item-preview">{{ chat.preview }}</div>
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
            <div class="suggestion-grid">
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
            </div>
          </div>

          <div
            v-for="message in messages"
            :key="message.id"
            :class="['message', message.role]"
          >
            <div class="message-wrapper">
              <div class="message-avatar">
                {{ message.role === "User" ? "User" : "AI" }}
              </div>

              <div class="message-content prose dark:prose-invert max-w-none">
                <div v-if="message.role === 'Assistant' && message.isTyping">
                  <div v-html="render(message.displayText)" />
                  <span
                    v-if="message.displayText !== message.content"
                    class="typing-cursor"
                  ></span>
                </div>
                <div v-else v-html="render(message.content)" />
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
              <textarea
                v-model="newMessage"
                @input="autoResize"
                placeholder="輸入訊息..."
                class="message-input"
                ref="messageInput"
                :disabled="isTyping"
                @keydown="handleKeyDown"
              ></textarea>
              <button
                :disabled="!newMessage.trim() || isTyping"
                class="send-button"
                @click="sendMessage"
              >
                ↑
              </button>
            </div>
            <div class="controls">
              <div class="char-count">{{ newMessage.length }}/2000</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import "~/assets/chatbot.css";
import { ref, onMounted, nextTick } from "vue";
import type { Conversation } from "~/types/conversation";
import {
  fetchChatHistory,
  fetchMessages,
  createConversation,
  askAI,
} from "~/composables/useChatApi";
import { useMarkdown } from "~/composables/useMarkdown";
const { render } = useMarkdown();

const messages = ref<any[]>([]);
const newMessage = ref("");
const isTyping = ref(false);
const chatHistory = ref<Conversation[]>([]);
const currentChatId = ref<number | null>(null);
const currentChatTitle = ref("Deepseek");
const loadingChats = ref(false);
const showSidebar = ref(false);
const userId = ref<string>("1");

const messageInput = ref<HTMLTextAreaElement | null>(null);
const messagesContainer = ref<HTMLDivElement | null>(null);

//for input
watch(newMessage, (newVal, oldVal) => {
  console.log("newMessage changed:", oldVal, "→", newVal);
});

watch(messages, (newVal, oldVal) => {
  console.log("messages updated. Total messages:", newVal);
});

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
    const history = await fetchChatHistory(userId.value);
    if (history.success) return (chatHistory.value = history.data ?? []);
  } catch (err) {
    console.error("網路或伺服器錯誤:", err);
  } finally {
    loadingChats.value = false;
  }
}

async function loadChat(conversationId: number) {
  try {
    const messageRes = await fetchMessages(conversationId);
    if (messageRes.success && messageRes.data) {
      currentChatId.value = messageRes.data.id;
      currentChatTitle.value = messageRes.data.title;
      messages.value = messageRes.data.messages;

      if (window.innerWidth <= 768) showSidebar.value = false;

      scrollToBottom();
    }
  } catch (err) {
    console.error("載入聊天失敗:", err);
  }
}

async function createNewChat() {
  try {
    const newChat = await createConversation(userId.value);
    if (!newChat.success) {
      throw new Error(
        `HTTP 錯誤碼: ${newChat.errorCode}, ${
          newChat.errorMessage ?? "API 回傳失敗"
        }`
      );
    }

    // ✅ 更新 UI 與狀態
    await loadChatHistory();

    currentChatId.value = newChat.data!.id;
    currentChatTitle.value = newChat.data!.title;
    messages.value = [];

    if (window.innerWidth <= 768) showSidebar.value = false;
    messageInput.value?.focus();
  } catch (err) {
    console.error("❌ 創建新聊天失敗:", err);
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

async function sendMessage() {
  // ✅ 檢查輸入訊息是否為空或正在輸入中
  if (!newMessage.value.trim() || isTyping.value) return;

  const content = newMessage.value.trim();
  isTyping.value = true;
  scrollToBottom();

  // ✅ 如果沒有 currentChatId，就創建一個新對話
  if (!currentChatId.value) {
    await createNewChat(); // ⚠️ 確保這個會更新 currentChatId.value
  }

  // ✅ 組合使用者訊息
  const userMessage = {
    role: "user",
    content: content,
  };

  try {
    const reply = await askAI(currentChatId.value!, [userMessage]);

    console.log("AI 回應完整資料", reply);
    if (!reply.success || !reply.data) {
      throw new Error(reply.errorMessage ?? "AI 回應失敗");
    }

    // ✅ 將使用者訊息加入 messages（可依結構調整）
    messages.value.push({
      role: "user",
      content: content,
    });

    // ✅ 加入 AI 回覆內容
    const aiContent = reply.data.aiResponse.choices[0]?.message.content;
    if (aiContent) {
      messages.value.push({
        role: "assistant",
        content: aiContent,
      });
    }
  } catch (err) {
    console.error("❌ 發送訊息失敗:", err);
    // 可以加上 toast 或顯示錯誤提示給使用者
  } finally {
    // ✅ 重置輸入欄與狀態
    newMessage.value = "";
    isTyping.value = false;
    scrollToBottom();
  }
}

function sendSuggestion(text: string) {
  newMessage.value = text;
  sendMessage();
}

function autoResize(event: Event) {
  const textarea = event.target as HTMLTextAreaElement;
  textarea.style.height = "auto";
  textarea.style.height = textarea.scrollHeight + "px";
}

function handleKeyDown(event: KeyboardEvent) {
  if (event.key === "Enter" && !event.shiftKey) {
    event.preventDefault();
    sendMessage();
  }
}

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
</script>
