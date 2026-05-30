import { defineStore } from 'pinia'
import { ref } from 'vue'
import * as api from '@/api/formTemplates'
import type {
  CreateFormTemplateRequest,
  ListFormTemplatesParams,
  FormTemplateDto,
  FormTemplateStatus,
  FormTemplateSummaryDto,
  UpdateFormTemplateRequest,
} from '@/types/formTemplates'

export const useFormTemplatesStore = defineStore('formTemplates', () => {
  const list = ref<FormTemplateSummaryDto[]>([])
  const totalCount = ref(0)
  const page = ref(1)
  const pageSize = ref(20)
  const activeForm = ref<FormTemplateDto | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchList(params: ListFormTemplatesParams = {}) {
    loading.value = true
    error.value = null
    try {
      const result = await api.listFormTemplates({
        page: page.value,
        pageSize: pageSize.value,
        ...params,
      })
      list.value = result.items
      totalCount.value = result.totalCount
      page.value = result.page
      pageSize.value = result.pageSize
    } catch (e) {
      error.value = extractMessage(e)
    } finally {
      loading.value = false
    }
  }

  async function fetchOne(id: string) {
    loading.value = true
    error.value = null
    try {
      activeForm.value = await api.getFormTemplate(id)
    } catch (e) {
      error.value = extractMessage(e)
    } finally {
      loading.value = false
    }
  }

  async function refreshActive(id: string) {
    const fresh = await api.getFormTemplate(id)
    activeForm.value = fresh
    const idx = list.value.findIndex((f) => f.id === id)
    if (idx !== -1) {
      list.value[idx] = {
        id: fresh.id,
        name: fresh.name,
        title: fresh.title,
        status: fresh.status,
        createdAt: fresh.createdAt,
        updatedAt: fresh.updatedAt,
      }
    }
  }

  async function create(req: CreateFormTemplateRequest): Promise<string | null> {
    error.value = null
    try {
      const id = await api.createFormTemplate(req)
      const fresh = await api.getFormTemplate(id)
      activeForm.value = fresh
      list.value = [
        {
          id: fresh.id,
          name: fresh.name,
          title: fresh.title,
          status: fresh.status,
          createdAt: fresh.createdAt,
          updatedAt: fresh.updatedAt,
        },
        ...list.value,
      ]
      totalCount.value += 1
      return id
    } catch (e) {
      error.value = extractMessage(e)
      return null
    }
  }

  async function rename(id: string, name: string): Promise<boolean> {
    error.value = null
    try {
      await api.renameFormTemplate(id, name)
      await refreshActive(id)
      return true
    } catch (e) {
      error.value = extractMessage(e)
      return false
    }
  }

  async function changeStatus(id: string, status: FormTemplateStatus): Promise<boolean> {
    error.value = null
    try {
      await api.changeFormTemplateStatus(id, status)
      await refreshActive(id)
      return true
    } catch (e) {
      error.value = extractMessage(e)
      return false
    }
  }

  async function updateForm(id: string, req: UpdateFormTemplateRequest): Promise<boolean> {
    error.value = null
    try {
      await api.updateFormTemplate(id, req)
      await refreshActive(id)
      return true
    } catch (e) {
      error.value = extractMessage(e)
      return false
    }
  }

  function extractMessage(e: unknown): string {
    if (e && typeof e === 'object' && 'detail' in e && e.detail) return String(e.detail)
    if (e && typeof e === 'object' && 'title' in e && e.title) return String(e.title)
    if (e instanceof Error) return e.message
    return 'An unexpected error occurred'
  }

  return {
    list,
    totalCount,
    page,
    pageSize,
    activeForm,
    loading,
    error,
    fetchList,
    fetchOne,
    create,
    rename,
    changeStatus,
    updateForm,
  }
})
