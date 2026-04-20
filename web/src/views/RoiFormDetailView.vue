<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useRoiFormsStore } from '@/stores/roiForms'
import type { RoiFormStatus } from '@/types/roiForms'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Tag from 'primevue/tag'

const route = useRoute()
const router = useRouter()
const toast = useToast()
const store = useRoiFormsStore()

const id = route.params.id as string
const editingName = ref('')
const saving = ref(false)

onMounted(async () => {
  await store.fetchOne(id)
  if (store.error) {
    toast.add({ severity: 'error', summary: 'Error', detail: store.error, life: 5000 })
  } else if (store.activeForm) {
    editingName.value = store.activeForm.name
  }
})

const form = computed(() => store.activeForm)

const nameChanged = computed(
  () => editingName.value.trim() !== form.value?.name && editingName.value.trim().length > 0,
)

async function saveName() {
  if (!nameChanged.value) return
  saving.value = true
  const updated = await store.rename(id, editingName.value.trim())
  saving.value = false
  if (updated) {
    toast.add({ severity: 'success', summary: 'Saved', detail: 'Name updated', life: 3000 })
  } else {
    toast.add({ severity: 'error', summary: 'Error', detail: store.error, life: 4000 })
  }
}

async function changeStatus(status: RoiFormStatus) {
  const updated = await store.changeStatus(id, status)
  if (updated) {
    toast.add({ severity: 'success', summary: 'Status updated', detail: `Status set to ${status}`, life: 3000 })
  } else {
    toast.add({ severity: 'error', summary: 'Error', detail: store.error, life: 4000 })
  }
}

function statusSeverity(status: RoiFormStatus): 'secondary' | 'success' | 'danger' {
  return status === 'Draft' ? 'secondary' : status === 'Published' ? 'success' : 'danger'
}

function formatDate(iso: string): string {
  return new Date(iso).toLocaleString()
}

function goBack() {
  router.push({ name: 'roi-forms' })
}
</script>

<template>
  <div class="roi-form-detail-page">
    <Button icon="pi pi-arrow-left" label="Back to Forms" severity="secondary" text @click="goBack" />

    <div v-if="store.loading" class="loading-state">Loading…</div>

    <div v-else-if="!form" class="error-state">
      <p>{{ store.error ?? 'Form not found.' }}</p>
      <Button label="Go back" @click="goBack" />
    </div>

    <template v-else>
      <div class="detail-card">
        <h2>ROI Form Details</h2>

        <div class="field">
          <label>Name</label>
          <div class="field-input-row">
            <InputText v-model="editingName" maxlength="200" @keyup.enter="saveName" />
            <Button
              label="Save"
              :disabled="!nameChanged"
              :loading="saving"
              @click="saveName"
            />
          </div>
        </div>

        <div class="field">
          <label>Status</label>
          <div class="status-row">
            <Tag :value="form.status" :severity="statusSeverity(form.status)" />
            <div class="action-buttons">
              <template v-if="form.status === 'Draft'">
                <Button
                  label="Publish"
                  severity="success"
                  @click="changeStatus('Published')"
                />
              </template>
              <template v-else-if="form.status === 'Published'">
                <Button
                  label="Set to Draft"
                  severity="secondary"
                  @click="changeStatus('Draft')"
                />
                <Button
                  label="Archive"
                  severity="danger"
                  @click="changeStatus('Archived')"
                />
              </template>
              <span v-else class="archived-note">Archived — no further transitions</span>
            </div>
          </div>
        </div>

        <div class="field">
          <label>Created</label>
          <span>{{ formatDate(form.createdAt) }}</span>
        </div>

        <div class="field">
          <label>Last updated</label>
          <span>{{ formatDate(form.updatedAt) }}</span>
        </div>

        <div class="field">
          <label>ID</label>
          <code>{{ form.id }}</code>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.roi-form-detail-page {
  padding: 2rem;
  max-width: 860px;
  margin: 0 auto;
  width: 100%;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.detail-card {
  border: 1px solid var(--p-surface-border, #e5e7eb);
  border-radius: 8px;
  padding: 2rem;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.detail-card h2 {
  margin: 0;
  font-size: 1.25rem;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
}

.field label {
  font-weight: 600;
  font-size: 0.875rem;
  color: var(--p-text-muted-color, #6b7280);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.field-input-row {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.field-input-row .p-inputtext {
  flex: 1;
}

.status-row {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

.archived-note {
  font-style: italic;
  color: var(--p-text-muted-color, #6b7280);
}

.loading-state,
.error-state {
  padding: 2rem;
  text-align: center;
  color: var(--p-text-muted-color, #6b7280);
}

code {
  font-family: monospace;
  font-size: 0.85rem;
  background: var(--p-surface-100, #f3f4f6);
  padding: 2px 6px;
  border-radius: 4px;
}
</style>
