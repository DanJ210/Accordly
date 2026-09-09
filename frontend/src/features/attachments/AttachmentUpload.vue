<template><label class="panel">Upload attachment<input type="file" @change="uploaded = true"><span v-if="uploaded" class="muted">Ready to upload</span></label></template>
<script setup lang="ts">import { ref } from 'vue'; const uploaded=ref(false)</script>
