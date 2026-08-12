<template>
  <div>
    <h1>ROI Form</h1>
    <p>This is the ROI form application.</p>
    <p>Counter: {{ counter }}</p>
  </div>
  <Button label="Verify" @click="counter++" />
</template>

<script setup>
import { ref } from 'vue'

const items = ref(["test1", "test2", "test3"])

const counter = ref(0);
</script>
