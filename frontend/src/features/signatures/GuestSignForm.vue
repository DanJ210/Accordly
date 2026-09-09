<template><form class="panel" @submit.prevent="signed = true"><p class="eyebrow">Signature</p><p>By signing, you confirm that you have reviewed this agreement.</p><button class="primary" type="submit">{{ signed ? 'Signed' : 'Sign agreement' }}</button></form></template>
<script setup lang="ts">import { ref } from 'vue'; const signed=ref(false)</script>
