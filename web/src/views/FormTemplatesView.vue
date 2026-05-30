<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useFormTemplatesStore } from '@/stores/formTemplates'
import type { FormTemplateSummaryDto, FormTemplateStatus } from '@/types/formTemplates'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Dialog from 'primevue/dialog'
import Tag from 'primevue/tag'
import type { DataTablePageEvent, DataTableSortEvent } from 'primevue/datatable'

const router = useRouter()
const toast = useToast()
const store = useFormTemplatesStore()

const nameFilter = ref('')
const showCreateDialog = ref(false)
const creating = ref(false)

const newForm = ref({ name: '', title: '', formula: '' })
const newFields = ref([{ key: '', label: '' }])

const sortField = ref('createdAt')
const sortOrder = ref<1 | -1>(-1)

async function loadPage(p: number = store.page) {
  await store.fetchList({
    name: nameFilter.value || undefined,
    sortBy: sortField.value as 'name' | 'createdAt' | 'status',
    sortDirection: sortOrder.value === 1 ? 'Ascending' : 'Descending',
    page: p,
    pageSize: store.pageSize,
  })
  if (store.error) toast.add({ severity: 'error', summary: 'Error', detail: store.error, life: 4000 })
}

onMounted(() => loadPage(1))

function onPage(event: DataTablePageEvent) {
  loadPage(event.page + 1)
}

function onSort(event: DataTableSortEvent) {
  sortField.value = event.sortField as string
  sortOrder.value = event.sortOrder as 1 | -1
  loadPage(1)
}

function onSearch() {
  loadPage(1)
}

function openDetail(form: FormTemplateSummaryDto) {
  router.push({ name: 'form-template-detail', params: { id: form.id } })
}

function openCreateDialog() {
  newForm.value = { name: '', title: '', formula: '' }
  newFields.value = [{ key: '', label: '' }]
  showCreateDialog.value = true
}

function addField() {
  newFields.value.push({ key: '', label: '' })
}

function removeField(index: number) {
  if (newFields.value.length > 1) newFields.value.splice(index, 1)
}

function isCreateValid() {
  return (
    newForm.value.name.trim() &&
    newForm.value.title.trim() &&
    newForm.value.formula.trim() &&
    newFields.value.length > 0 &&
    newFields.value.every((f) => f.key.trim() && f.label.trim())
  )
}

async function submitCreate() {
  if (!isCreateValid()) return
  creating.value = true
  const id = await store.create({
    name: newForm.value.name.trim(),
    title: newForm.value.title.trim(),
    formula: newForm.value.formula.trim(),
    formFields: newFields.value.map((f) => ({ key: f.key.trim(), label: f.label.trim() })),
  })
  creating.value = false
  if (id) {
    showCreateDialog.value = false
    toast.add({ severity: 'success', summary: 'Created', detail: `"${newForm.value.name}" created`, life: 3000 })
    router.push({ name: 'form-template-detail', params: { id } })
  } else {
    toast.add({ severity: 'error', summary: 'Error', detail: store.error, life: 4000 })
  }
}

async function changeStatus(form: FormTemplateSummaryDto, status: FormTemplateStatus) {
  const ok = await store.changeStatus(form.id, status)
  if (ok) {
    toast.add({ severity: 'success', summary: 'Status updated', detail: `Status set to ${status}`, life: 3000 })
  } else {
    toast.add({ severity: 'error', summary: 'Error', detail: store.error, life: 4000 })
  }
}

function statusSeverity(status: FormTemplateStatus): 'secondary' | 'success' | 'danger' {
  return status === 'Draft' ? 'secondary' : status === 'Published' ? 'success' : 'danger'
}

function formatDate(iso: string): string {
  return new Date(iso).toLocaleString()
}
</script>

<template>
  <div class="roi-forms-page">
    <div class="roi-forms-toolbar">
      <div class="roi-forms-search">
        <InputText v-model="nameFilter" placeholder="Search by name…" @keyup.enter="onSearch" />
        <Button label="Search" @click="onSearch" />
      </div>
      <Button label="New Form" icon="pi pi-plus" @click="openCreateDialog" />
    </div>

    <DataTable
      :value="store.list"
      :loading="store.loading"
      lazy
      paginator
      :rows="store.pageSize"
      :totalRecords="store.totalCount"
      :sortField="sortField"
      :sortOrder="sortOrder"
      @page="onPage"
      @sort="onSort"
      removableSort
      emptyMessage="No ROI forms found."
    >
      <Column field="name" header="Name" sortable>
        <template #body="{ data }">
          <a class="form-name-link" @click="openDetail(data)">{{ data.name }}</a>
        </template>
      </Column>

      <Column field="title" header="Title" />

      <Column field="status" header="Status" sortable>
        <template #body="{ data }">
          <Tag :value="data.status" :severity="statusSeverity(data.status)" />
        </template>
      </Column>

      <Column field="createdAt" header="Created" sortable>
        <template #body="{ data }">{{ formatDate(data.createdAt) }}</template>
      </Column>

      <Column header="Actions">
        <template #body="{ data }">
          <div class="action-buttons">
            <template v-if="data.status === 'Draft'">
              <Button label="Publish" size="small" severity="success" @click="changeStatus(data, 'Published')" />
            </template>
            <template v-else-if="data.status === 'Published'">
              <Button label="Set to Draft" size="small" severity="secondary" @click="changeStatus(data, 'Draft')" />
              <Button label="Archive" size="small" severity="danger" @click="changeStatus(data, 'Archived')" />
            </template>
          </div>
        </template>
      </Column>
    </DataTable>

    <Dialog v-model:visible="showCreateDialog" header="New ROI Form" modal :style="{ width: '560px' }">
      <div class="create-dialog-body">
        <div class="field-group">
          <label for="new-name">Name</label>
          <InputText id="new-name" v-model="newForm.name" placeholder="e.g. ROI Calculator" maxlength="200" />
        </div>

        <div class="field-group">
          <label for="new-title">Title</label>
          <InputText id="new-title" v-model="newForm.title" placeholder="e.g. Calculate Return on Investment" maxlength="200" />
        </div>

        <div class="field-group">
          <label for="new-formula">Formula</label>
          <Textarea
            id="new-formula"
            v-model="newForm.formula"
            placeholder="e.g. (revenue - cost) / cost * 100"
            :maxlength="4000"
            rows="2"
            autoResize
          />
          <span class="field-hint">Use field keys as variables in the expression.</span>
        </div>

        <div class="field-group">
          <label>Form Fields</label>
          <div class="fields-list">
            <div v-for="(field, i) in newFields" :key="i" class="fields-row">
              <InputText v-model="field.key" placeholder="Key (e.g. revenue)" maxlength="200" class="field-key" />
              <InputText v-model="field.label" placeholder="Label (e.g. Revenue)" maxlength="200" class="field-label" />
              <Button
                icon="pi pi-trash"
                severity="danger"
                text
                :disabled="newFields.length === 1"
                @click="removeField(i)"
              />
            </div>
          </div>
          <Button label="Add Field" icon="pi pi-plus" severity="secondary" size="small" text @click="addField" />
        </div>
      </div>

      <template #footer>
        <Button label="Cancel" severity="secondary" @click="showCreateDialog = false" />
        <Button label="Create" :loading="creating" :disabled="!isCreateValid()" @click="submitCreate" />
      </template>
    </Dialog>
  </div>
</template>

<style scoped>
.roi-forms-page {
  padding: 2rem;
  width: 100%;
  box-sizing: border-box;
}

.roi-forms-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  gap: 1rem;
  flex-wrap: wrap;
}

.roi-forms-search {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.form-name-link {
  cursor: pointer;
  color: var(--p-primary-color, #4f46e5);
  text-decoration: underline;
}

.form-name-link:hover {
  opacity: 0.8;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

.create-dialog-body {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 0.25rem 0;
}

.field-group {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
}

.field-group label {
  font-weight: 600;
  font-size: 0.875rem;
}

.field-group .p-inputtext,
.field-group .p-textarea {
  width: 100%;
}

.field-hint {
  font-size: 0.8rem;
  color: var(--p-text-muted-color, #6b7280);
}

.fields-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 0.25rem;
}

.fields-row {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.field-key {
  flex: 1;
}

.field-label {
  flex: 2;
}
</style>
