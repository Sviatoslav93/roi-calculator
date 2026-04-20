import { defineStore } from 'pinia'
import { ref } from 'vue'
import * as api from '@/api/roiForms'
import type { ListRoiFormsParams, RoiFormDto, RoiFormStatus } from '@/types/roiForms'

export const useRoiFormsStore = defineStore('roiForms', () => {
  const list = ref<RoiFormDto[]>([])
  const totalCount = ref(0)
  const page = ref(1)
  const pageSize = ref(20)
  const activeForm = ref<RoiFormDto | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchList(params: ListRoiFormsParams = {}) {
    loading.value = true
    error.value = null
    try {
      const result = await api.listRoiForms({
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
      activeForm.value = await api.getRoiForm(id)
    } catch (e) {
      error.value = extractMessage(e)
    } finally {
      loading.value = false
    }
  }

  async function create(name: string): Promise<RoiFormDto | null> {
    error.value = null
    try {
      const form = await api.createRoiForm(name)
      list.value = [form, ...list.value]
      totalCount.value += 1
      return form
    } catch (e) {
      error.value = extractMessage(e)
      return null
    }
  }

  async function rename(id: string, name: string): Promise<RoiFormDto | null> {
    error.value = null
    try {
      const updated = await api.updateRoiFormName(id, name)
      replaceInList(updated)
      if (activeForm.value?.id === id) activeForm.value = updated
      return updated
    } catch (e) {
      error.value = extractMessage(e)
      return null
    }
  }

  async function changeStatus(id: string, status: RoiFormStatus): Promise<RoiFormDto | null> {
    error.value = null
    try {
      const updated = await api.updateRoiFormStatus(id, status)
      replaceInList(updated)
      if (activeForm.value?.id === id) activeForm.value = updated
      return updated
    } catch (e) {
      error.value = extractMessage(e)
      return null
    }
  }

  function replaceInList(updated: RoiFormDto) {
    const idx = list.value.findIndex((f) => f.id === updated.id)
    if (idx !== -1) list.value[idx] = updated
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
  }
})
