import { useState } from 'react'
import type { ChangeEvent } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Link, useNavigate } from 'react-router-dom'
import { ApiError, criarCandidato, extrairCurriculo } from '../api'
import { candidatoSchema } from '../schema'
import type { CandidatoForm } from '../schema'

type Aviso = { tipo: 'sucesso' | 'erro' | 'info'; texto: string }

const TAMANHO_MAXIMO = 5 * 1024 * 1024

export default function NovoPage() {
  const navigate = useNavigate()
  const [aviso, setAviso] = useState<Aviso | null>(null)
  const [lendoPdf, setLendoPdf] = useState(false)

  const { register, handleSubmit, setValue, setError, formState: { errors, isSubmitting } } =
    useForm<CandidatoForm>({
      resolver: zodResolver(candidatoSchema),
      defaultValues: { nomeCompleto: '', email: '', telefone: '', areaInteresse: '', resumoProfissional: '' },
    })

  async function importarPdf(e: ChangeEvent<HTMLInputElement>) {
    const arquivo = e.target.files?.[0]
    e.target.value = '' // permite escolher o mesmo arquivo de novo
    if (!arquivo) return

    if (!arquivo.name.toLowerCase().endsWith('.pdf')) {
      setAviso({ tipo: 'erro', texto: 'Formato inválido. Envie um arquivo PDF.' })
      return
    }
    if (arquivo.size > TAMANHO_MAXIMO) {
      setAviso({ tipo: 'erro', texto: 'O arquivo excede o limite de 5 MB.' })
      return
    }

    setLendoPdf(true)
    setAviso(null)
    try {
      const dados = await extrairCurriculo(arquivo)
      const encontrados: string[] = []

      if (dados.nomeCompleto) { setValue('nomeCompleto', dados.nomeCompleto, { shouldValidate: true }); encontrados.push('nome') }
      if (dados.email) { setValue('email', dados.email, { shouldValidate: true }); encontrados.push('e-mail') }
      if (dados.telefone) { setValue('telefone', dados.telefone, { shouldValidate: true }); encontrados.push('telefone') }

      setAviso(encontrados.length > 0
        ? { tipo: 'info', texto: `Encontramos: ${encontrados.join(', ')}. Confira e complete os demais campos antes de salvar.` }
        : { tipo: 'info', texto: 'Não identificamos dados no PDF. Preencha o formulário manualmente.' })
    } catch (err) {
      const texto = err instanceof ApiError ? err.message : 'Falha ao ler o PDF.'
      setAviso({ tipo: 'erro', texto: `${texto} Você pode continuar o cadastro manualmente.` })
    } finally {
      setLendoPdf(false)
    }
  }

  async function salvar(dados: CandidatoForm) {
    setAviso(null)
    try {
      const criado = await criarCandidato(dados)
      navigate(`/candidatos/${criado.id}`, { state: { criado: true } })
    } catch (err) {
      if (!(err instanceof ApiError)) {
        setAviso({ tipo: 'erro', texto: 'Ocorreu um erro inesperado.' })
        return
      }
      // Erros de validação do backend vêm como { Email: [...], NomeCompleto: [...] }
      for (const [campo, mensagens] of Object.entries(err.fieldErrors)) {
        const nome = (campo.charAt(0).toLowerCase() + campo.slice(1)) as keyof CandidatoForm
        setError(nome, { message: mensagens[0] })
      }
      if (err.status === 409) setError('email', { message: err.message })
      setAviso({ tipo: 'erro', texto: err.message })
    }
  }

  return (
    <>
      <Link to="/">← Voltar para a lista</Link>
      <h1>Novo candidato</h1>

      <section className="card">
        <label htmlFor="pdf"><strong>Importar de um currículo em PDF</strong> (opcional, até 5 MB)</label>
        <input id="pdf" type="file" accept=".pdf,application/pdf" onChange={importarPdf} disabled={lendoPdf} />
        {lendoPdf && <p>Lendo currículo...</p>}
      </section>

      {aviso && <p className={`aviso ${aviso.tipo}`} role="alert">{aviso.texto}</p>}

      <form onSubmit={handleSubmit(salvar)} noValidate className="card">
        <label htmlFor="nomeCompleto">Nome completo *</label>
        <input id="nomeCompleto" {...register('nomeCompleto')} />
        {errors.nomeCompleto && <span className="erro-campo">{errors.nomeCompleto.message}</span>}

        <label htmlFor="email">E-mail *</label>
        <input id="email" type="email" {...register('email')} />
        {errors.email && <span className="erro-campo">{errors.email.message}</span>}

        <label htmlFor="telefone">Telefone</label>
        <input id="telefone" {...register('telefone')} />
        {errors.telefone && <span className="erro-campo">{errors.telefone.message}</span>}

        <label htmlFor="areaInteresse">Área ou cargo de interesse</label>
        <input id="areaInteresse" {...register('areaInteresse')} />
        {errors.areaInteresse && <span className="erro-campo">{errors.areaInteresse.message}</span>}

        <label htmlFor="resumoProfissional">Resumo profissional</label>
        <textarea id="resumoProfissional" rows={5} {...register('resumoProfissional')} />
        {errors.resumoProfissional && <span className="erro-campo">{errors.resumoProfissional.message}</span>}

        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Salvando...' : 'Salvar candidato'}
        </button>
      </form>
    </>
  )
}