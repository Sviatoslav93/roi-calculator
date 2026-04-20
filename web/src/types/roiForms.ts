export type RoiFormStatus = 'Draft' | 'Published' | 'Archived'

export interface RoiFormDto {
    id: string
    name: string
    status: RoiFormStatus
    createdAt: string
    updatedAt: string
}

export interface PagedResult<T> {
    items: T[]
    totalCount: number
    page: number
    pageSize: number
}

export interface ListRoiFormsParams {
    name?: string
    sortBy?: 'name' | 'createdAt' | 'status'
    sortDirection?: 'Ascending' | 'Descending'
    page?: number
    pageSize?: number
}
