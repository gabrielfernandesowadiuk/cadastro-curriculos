import type { CandidatoForm } from './schema'

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5062'

export interface CandidatoResumo {
  id: number
  nomeCompleto: string
  email: string
  areaInteresse: string | null
  criadoEm: string
}

export interface Candidato extends CandidatoResumo {
  telefone: string | null
  resumoProfissional: string | null
}

export interface Extracao {
  nomeCompleto: string | null
  email: string | null
  telefone: string | null
}

export class ApiError extends Error {
  status: number
  fieldErrors: Record<string, string[]>

  constructor(message: string, status: number, fieldErrors: Record<string, string[]> = {}) {
    super(message)
    this.status = status
    this.fieldErrors = fieldErrors
  }
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let res: Response
  try {
    res = await fetch(`${API_URL}${path}`, init)
  } catch {
    throw new ApiError('Não foi possível conectar ao servidor. Verifique se a API está em execução.', 0)
  }

  if (!res.ok) {
    const body = await res.json().catch(() => null)
    let message = 'Ocorreu um erro inesperado.'
    if (body?.errors) message = 'Corrija os campos destacados.'
    else if (body?.detail || body?.title) message = body.detail ?? body.title
    else if (res.status === 413) message = 'O arquivo excede o limite de 5 MB.'
    throw new ApiError(message, res.status, body?.errors ?? {})
  }

  return res.json() as Promise<T>
}

export const listarCandidatos = () => request<CandidatoResumo[]>('/api/candidatos')

export const obterCandidato = (id: number) => request<Candidato>(`/api/candidatos/${id}`)

export const criarCandidato = (dados: CandidatoForm) =>
  request<Candidato>('/api/candidatos', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dados),
  })

export function extrairCurriculo(arquivo: File) {
  const form = new FormData()
  form.append('arquivo', arquivo)
  return request<Extracao>('/api/curriculos/extrair', { method: 'POST', body: form })
}

// O banco grava em UTC sem o "Z"; adicionamos para o navegador converter para o horário local.
export const formatarData = (iso: string) =>
  new Date(iso.endsWith('Z') ? iso : `${iso}Z`).toLocaleString('pt-BR')