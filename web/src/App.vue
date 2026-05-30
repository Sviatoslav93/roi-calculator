<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import Toast from 'primevue/toast'
</script>

<template>
  <header class="app-header">
    <div class="app-header-inner">
      <RouterLink to="/form-templates" class="app-brand">ROI Calculator</RouterLink>
      <nav>
        <RouterLink to="/form-templates">Form Templates</RouterLink>
      </nav>
    </div>
  </header>

  <Toast />
  <main class="app-main">
    <RouterView />
  </main>
</template>

<style scoped>
.app-header {
  border-bottom: 1px solid var(--color-border);
  background: var(--color-background);
  position: sticky;
  top: 0;
  z-index: 100;
}

.app-header-inner {
  max-width: 1100px;
  margin: 0 auto;
  padding: 0 2rem;
  height: 56px;
  display: flex;
  align-items: center;
  gap: 2rem;
}

.app-brand {
  font-weight: 700;
  font-size: 1.1rem;
  text-decoration: none;
  color: var(--color-heading);
}

nav a {
  font-size: 0.95rem;
  color: var(--color-text);
  text-decoration: none;
  padding: 0.25rem 0;
  border-bottom: 2px solid transparent;
}

nav a.router-link-active {
  color: var(--color-heading);
  border-bottom-color: currentColor;
}

.app-main {
  flex: 1;
  display: flex;
  flex-direction: column;
}
</style>
