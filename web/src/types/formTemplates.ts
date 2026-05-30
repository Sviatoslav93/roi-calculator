export type FormTemplateStatus = 'Draft' | 'Published' | 'Archived'

export type FormFieldType = 'Text' | 'Number' | 'Email' | 'Slider'

export interface FormFieldDto {
  key: string
  label: string
  type: FormFieldType
  min?: number
  max?: number
}

export interface FormTemplateSummaryDto {
  id: string
  name: string
  title: string
  status: FormTemplateStatus
  createdAt: string
  updatedAt: string | null
}

export interface FormTemplateDto {
  id: string
  name: string
  title: string
  status: FormTemplateStatus
  formula: string
  formFields: FormFieldDto[]
  createdAt: string
  updatedAt: string | null
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
}

export interface ListFormTemplatesParams {
  name?: string
  sortBy?: 'name' | 'createdAt' | 'status'
  sortDirection?: 'Ascending' | 'Descending'
  page?: number
  pageSize?: number
}

export interface CreateFormTemplateRequest {
  name: string
  title: string
  formula: string
  formFields: { key: string; label: string; type: FormFieldType; min?: number; max?: number }[]
}

export interface UpdateFormTemplateRequest {
  title: string
  formula: string
  formFields: { key: string; label: string; type: FormFieldType; min?: number; max?: number }[]
}
