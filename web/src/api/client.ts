const baseUrl = import.meta.env.VITE_API_BASE_URL as string

export interface ApiError {
  status: number
  title?: string
  detail?: string
}

async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  })

  if (response.status === 204 || response.headers.get('content-length') === '0') {
    return undefined as T
  }

  const body = response.headers.get('content-type')?.includes('application/json')
    ? await response.json()
    : await response.text()

  if (!response.ok) {
    const error: ApiError = {
      status: response.status,
      title: body?.title,
      detail: body?.detail,
    }
    throw error
  }

  return body as T
}

export default apiFetch
