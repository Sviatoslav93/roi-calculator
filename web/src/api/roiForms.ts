import apiFetch from './client'
import type { ListRoiFormsParams, PagedResult, RoiFormDto, RoiFormStatus } from '@/types/roiForms'

export function createRoiForm(name: string): Promise<RoiFormDto> {
  return apiFetch<RoiFormDto>('/roi-forms', {
    method: 'POST',
    body: JSON.stringify({ name }),
  })
}

export function getRoiForm(id: string): Promise<RoiFormDto> {
  return apiFetch<RoiFormDto>(`/roi-forms/${id}`)
}

export function listRoiForms(params: ListRoiFormsParams = {}): Promise<PagedResult<RoiFormDto>> {
  const query = new URLSearchParams()
  if (params.name) query.set('name', params.name)
  if (params.sortBy) query.set('sortBy', params.sortBy)
  if (params.sortDirection) query.set('sortDirection', params.sortDirection)
  if (params.page !== undefined) query.set('page', String(params.page))
  if (params.pageSize !== undefined) query.set('pageSize', String(params.pageSize))
  const qs = query.toString()
  return apiFetch<PagedResult<RoiFormDto>>(`/roi-forms${qs ? `?${qs}` : ''}`)
}

export function updateRoiFormName(id: string, name: string): Promise<RoiFormDto> {
  return apiFetch<RoiFormDto>(`/roi-forms/${id}/name`, {
    method: 'PATCH',
    body: JSON.stringify({ name }),
  })
}

export function updateRoiFormStatus(id: string, status: RoiFormStatus): Promise<RoiFormDto> {
  return apiFetch<RoiFormDto>(`/roi-forms/${id}/status`, {
    method: 'PATCH',
    body: JSON.stringify({ status }),
  })
}
