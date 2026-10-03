import { useEffect, useState } from 'react'
import { Link, useLocation, useParams } from 'react-router-dom'
import { ApiError, formatarData, obterCandidato } from '../api'
import type { Candidato } from '../api'

export default function DetalhePage() {
  const { id } = useParams()
  const state = useLocation().state as { criado?: boolean } | null
  const [candidato, setCandidato] = useState<Candidato | null>(null)
  const [erro, setErro] = useState<string | null>(null)

  useEffect(() => {
    obterCandidato(Number(id))
      .then(setCandidato)
      .catch((e: unknown) => setErro(e instanceof ApiError ? e.message : 'Erro inesperado.'))
  }, [id])

  return (
    <>
      <Link to="/">← Voltar para a lista</Link>
      <h1>Detalhes do candidato</h1>

      {state?.criado && <p className="aviso sucesso" role="status">Candidato cadastrado com sucesso.</p>}
      {erro && <p className="aviso erro" role="alert">{erro}</p>}
      {!erro && !candidato && <p>Carregando...</p>}

      {candidato && (
        <dl className="card">
          <dt>Nome completo</dt><dd>{candidato.nomeCompleto}</dd>
          <dt>E-mail</dt><dd>{candidato.email}</dd>
          <dt>Telefone</dt><dd>{candidato.telefone ?? '—'}</dd>
          <dt>Área ou cargo de interesse</dt><dd>{candidato.areaInteresse ?? '—'}</dd>
          <dt>Resumo profissional</dt><dd className="resumo">{candidato.resumoProfissional ?? '—'}</dd>
          <dt>Cadastrado em</dt><dd>{formatarData(candidato.criadoEm)}</dd>
        </dl>
      )}
    </>
  )
}