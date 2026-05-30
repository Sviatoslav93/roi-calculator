import apiFetch from './client'
import type {
  CreateFormTemplateRequest,
  ListFormTemplatesParams,
  PagedResult,
  FormTemplateDto,
  FormTemplateStatus,
  FormTemplateSummaryDto,
  UpdateFormTemplateRequest,
} from '@/types/formTemplates'

export function createFormTemplate(req: CreateFormTemplateRequest): Promise<string> {
  return apiFetch<string>('/form-templates', {
    method: 'POST',
    body: JSON.stringify({
      ...req,
      formFields: req.formFields.map((f) => ({ ...f, type: 2 })),
    }),
  })
}

export function getFormTemplate(id: string): Promise<FormTemplateDto> {
  return apiFetch<FormTemplateDto>(`/form-templates/${id}`)
}

export function listFormTemplates(params: ListFormTemplatesParams = {}): Promise<PagedResult<FormTemplateSummaryDto>> {
  const query = new URLSearchParams()
  if (params.name) query.set('name', params.name)
  if (params.sortBy) query.set('sortBy', params.sortBy)
  if (params.sortDirection) query.set('sortDirection', params.sortDirection)
  if (params.page !== undefined) query.set('page', String(params.page))
  if (params.pageSize !== undefined) query.set('pageSize', String(params.pageSize))
  const qs = query.toString()
  return apiFetch<PagedResult<FormTemplateSummaryDto>>(`/form-templates${qs ? `?${qs}` : ''}`)
}

export function updateFormTemplate(id: string, req: UpdateFormTemplateRequest): Promise<void> {
  return apiFetch<void>(`/form-templates/${id}`, {
    method: 'PATCH',
    body: JSON.stringify(req),
  })
}

export function renameFormTemplate(id: string, name: string): Promise<void> {
  return apiFetch<void>(`/form-templates/${id}/name`, {
    method: 'PATCH',
    body: JSON.stringify({ name }),
  })
}

export function changeFormTemplateStatus(id: string, status: FormTemplateStatus): Promise<void> {
  return apiFetch<void>(`/form-templates/${id}/status`, {
    method: 'PATCH',
    body: JSON.stringify({ status }),
  })
}
