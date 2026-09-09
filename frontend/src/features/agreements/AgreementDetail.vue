<template><article><p class="eyebrow">Agreement</p><h1>{{ agreement.title }}</h1><span class="status">{{ agreement.status }}</span><slot /></article></template>
<script setup lang="ts">import type { Agreement } from '@/stores/agreements'; defineProps<{ agreement: Agreement }>()</script>
