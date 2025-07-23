<template>
  <button class="btn-primary">
    <slot>按鈕</slot>
  </button>
</template>

<script setup>
// 可以寫一些邏輯或 props
</script>

<style scoped>
.btn-primary {
  background-color: #007bff;
  color: white;
  padding: 8px 16px;
  border-radius: 4px;
}
</style>
