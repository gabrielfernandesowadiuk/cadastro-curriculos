import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { ApiError, formatarData, listarCandidatos } from '../api'
import type { CandidatoResumo } from '../api'

export default function ListaPage() {
  const [candidatos, setCandidatos] = useState<CandidatoResumo[] | null>(null)
  const [erro, setErro] = useState<string | null>(null)

  useEffect(() => {
    listarCandidatos()
      .then(setCandidatos)
      .catch((e: unknown) => setErro(e instanceof ApiError ? e.message : 'Erro inesperado.'))
  }, [])

  return (
    <>
      <header className="topo">
        <h1>Candidatos</h1>
        <Link className="botao" to="/candidatos/novo">Novo candidato</Link>
      </header>

      {erro && <p className="aviso erro" role="alert">{erro}</p>}
      {!erro && candidatos === null && <p>Carregando...</p>}
      {candidatos?.length === 0 && <p>Nenhum candidato cadastrado ainda.</p>}

      {candidatos && candidatos.length > 0 && (
        <table>
          <thead>
            <tr><th>Nome</th><th>E-mail</th><th>Área de interesse</th><th>Cadastrado em</th></tr>
          </thead>
          <tbody>
            {candidatos.map(c => (
              <tr key={c.id}>
                <td><Link to={`/candidatos/${c.id}`}>{c.nomeCompleto}</Link></td>
                <td>{c.email}</td>
                <td>{c.areaInteresse ?? '—'}</td>
                <td>{formatarData(c.criadoEm)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  )
}