<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import DefaultLayout from './layouts/DefaultLayout.vue'
import GuestLayout from './layouts/GuestLayout.vue'
const route = useRoute()
const guest = computed(() => ['/login', '/register'].includes(route.path) || route.path.startsWith('/sign/'))
</script>
<template><component :is="guest ? GuestLayout : DefaultLayout"><RouterView /></component></template>
