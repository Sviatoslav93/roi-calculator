<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useFormTemplatesStore } from '@/stores/formTemplates'
import type { FormTemplateStatus } from '@/types/formTemplates'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Tag from 'primevue/tag'

const route = useRoute()
const router = useRouter()
const toast = useToast()
const store = useFormTemplatesStore()

const id = route.params.id as string

const editingName = ref('')
const savingName = ref(false)

const editTitle = ref('')
const editFormula = ref('')
const editFields = ref<{ key: string; label: string }[]>([])
const savingContent = ref(false)

onMounted(async () => {
  await store.fetchOne(id)
  if (store.error) {
    toast.add({ severity: 'error', summary: 'Error', detail: store.error, life: 5000 })
  } else if (store.activeForm) {
    syncEditors()
  }
})

const form = computed(() => store.activeForm)

function syncEditors() {
  if (!form.value) return
  editingName.value = form.value.name
  editTitle.value = form.value.title
  editFormula.value = form.value.formula
  editFields.value = form.value.formFields.map((f) => ({ key: f.key, label: f.label }))
}

const nameChanged = computed(
  () => editingName.value.trim() !== form.value?.name && editingName.value.trim().length > 0,
)

const contentChanged = computed(() => {
  if (!form.value) return false
  if (editTitle.value.trim() !== form.value.title) return true
  if (editFormula.value.trim() !== form.value.formula) return true
  const orig = form.value.formFields.map((f) => `${f.key}|${f.label}`).join(',')
  const curr = editFields.value.map((f) => `${f.key.trim()}|${f.label.trim()}`).join(',')
  return orig !== curr
})

const contentValid = computed(
  () =>
    editTitle.value.trim() &&
    editFormula.value.trim() &&
    editFields.value.length > 0 &&
    editFields.value.every((f) => f.key.trim() && f.label.trim()),
)

async function saveName() {
  if (!nameChanged.value) return
  savingName.value = true
  const ok = await store.rename(id, editingName.value.trim())
  savingName.value = false
  if (ok) {
    toast.add({ severity: 'success', summary: 'Saved', detail: 'Name updated', life: 3000 })
  } else {
    toast.add({ severity: 'error', summary: 'Error', detail: store.error, life: 4000 })
  }
}

async function saveContent() {
  if (!contentValid.value) return
  savingContent.value = true
  const ok = await store.updateForm(id, {
    title: editTitle.value.trim(),
    formula: editFormula.value.trim(),
    formFields: editFields.value.map((f) => ({ key: f.key.trim(), label: f.label.trim() })),
  })
  savingContent.value = false
  if (ok) {
    toast.add({ severity: 'success', summary: 'Saved', detail: 'Form updated', life: 3000 })
  } else {
    toast.add({ severity: 'error', summary: 'Error', detail: store.error, life: 4000 })
  }
}

async function changeStatus(status: FormTemplateStatus) {
  const ok = await store.changeStatus(id, status)
  if (ok) {
    toast.add({ severity: 'success', summary: 'Status updated', detail: `Status set to ${status}`, life: 3000 })
  } else {
    toast.add({ severity: 'error', summary: 'Error', detail: store.error, life: 4000 })
  }
}

function addField() {
  editFields.value.push({ key: '', label: '' })
}

function removeField(index: number) {
  if (editFields.value.length > 1) editFields.value.splice(index, 1)
}

function statusSeverity(status: FormTemplateStatus): 'secondary' | 'success' | 'danger' {
  return status === 'Draft' ? 'secondary' : status === 'Published' ? 'success' : 'danger'
}

function formatDate(iso: string | null): string {
  return iso ? new Date(iso).toLocaleString() : '—'
}

function goBack() {
  router.push({ name: 'form-templates' })
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
      <!-- Name -->
      <div class="detail-card">
        <div class="card-section">
          <div class="field">
            <label>Name</label>
            <div class="field-input-row">
              <InputText v-model="editingName" maxlength="200" @keyup.enter="saveName" />
              <Button label="Save" :disabled="!nameChanged" :loading="savingName" @click="saveName" />
            </div>
          </div>
        </div>

        <!-- Status -->
        <div class="card-section">
          <div class="field">
            <label>Status</label>
            <div class="status-row">
              <Tag :value="form.status" :severity="statusSeverity(form.status)" />
              <div class="action-buttons">
                <template v-if="form.status === 'Draft'">
                  <Button label="Publish" severity="success" @click="changeStatus('Published')" />
                </template>
                <template v-else-if="form.status === 'Published'">
                  <Button label="Set to Draft" severity="secondary" @click="changeStatus('Draft')" />
                  <Button label="Archive" severity="danger" @click="changeStatus('Archived')" />
                </template>
                <span v-else class="archived-note">Archived — no further transitions</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Title, Formula, Fields -->
        <div class="card-section">
          <div class="field">
            <label>Title</label>
            <InputText v-model="editTitle" maxlength="200" placeholder="Display title" />
          </div>

          <div class="field">
            <label>Formula</label>
            <Textarea
              v-model="editFormula"
              :maxlength="4000"
              rows="2"
              autoResize
              placeholder="e.g. (revenue - cost) / cost * 100"
            />
            <span class="field-hint">Use field keys as variables.</span>
          </div>

          <div class="field">
            <label>Form Fields</label>
            <div class="fields-list">
              <div class="fields-header">
                <span class="col-key">Key</span>
                <span class="col-label">Label</span>
                <span class="col-remove"></span>
              </div>
              <div v-for="(f, i) in editFields" :key="i" class="fields-row">
                <InputText v-model="f.key" placeholder="key" maxlength="200" class="col-key" />
                <InputText v-model="f.label" placeholder="Label" maxlength="200" class="col-label" />
                <Button
                  icon="pi pi-trash"
                  severity="danger"
                  text
                  :disabled="editFields.length === 1"
                  class="col-remove"
                  @click="removeField(i)"
                />
              </div>
            </div>
            <Button label="Add Field" icon="pi pi-plus" severity="secondary" size="small" text @click="addField" />
          </div>

          <div class="save-row">
            <Button
              label="Save Changes"
              :disabled="!contentChanged || !contentValid"
              :loading="savingContent"
              @click="saveContent"
            />
          </div>
        </div>

        <!-- Timestamps -->
        <div class="card-section timestamps">
          <div class="field inline">
            <label>Created</label>
            <span>{{ formatDate(form.createdAt) }}</span>
          </div>
          <div class="field inline">
            <label>Last updated</label>
            <span>{{ formatDate(form.updatedAt) }}</span>
          </div>
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
  overflow: hidden;
}

.card-section {
  padding: 1.5rem 2rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.card-section + .card-section {
  border-top: 1px solid var(--p-surface-border, #e5e7eb);
}

.timestamps {
  background: var(--p-surface-50, #f9fafb);
  flex-direction: row;
  gap: 2rem;
  flex-wrap: wrap;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
}

.field.inline {
  flex-direction: row;
  align-items: center;
  gap: 0.5rem;
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

.field .p-inputtext,
.field .p-textarea {
  width: 100%;
}

.field-hint {
  font-size: 0.8rem;
  color: var(--p-text-muted-color, #6b7280);
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
  align-items: center;
}

.archived-note {
  font-style: italic;
  color: var(--p-text-muted-color, #6b7280);
}

.fields-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.fields-header {
  display: flex;
  gap: 0.5rem;
  font-size: 0.75rem;
  font-weight: 600;
  color: var(--p-text-muted-color, #6b7280);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  padding: 0 0.25rem;
}

.fields-row {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.col-key {
  flex: 1;
}

.col-label {
  flex: 2;
}

.col-remove {
  flex-shrink: 0;
  width: 2.5rem;
}

.save-row {
  display: flex;
  justify-content: flex-end;
}

.loading-state,
.error-state {
  padding: 2rem;
  text-align: center;
  color: var(--p-text-muted-color, #6b7280);
}
</style>
