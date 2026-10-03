import { z } from 'zod'

export const candidatoSchema = z.object({
  nomeCompleto: z.string().trim()
    .min(1, 'O nome completo é obrigatório.')
    .max(150, 'O nome pode ter no máximo 150 caracteres.'),
  email: z.string().trim()
    .min(1, 'O e-mail é obrigatório.')
    .max(150, 'O e-mail pode ter no máximo 150 caracteres.')
    .regex(/^[^@\s]+@[^@\s]+\.[^@\s]+$/, 'Informe um e-mail válido.'),
  telefone: z.string().max(20, 'O telefone pode ter no máximo 20 caracteres.'),
  areaInteresse: z.string().max(100, 'A área pode ter no máximo 100 caracteres.'),
  resumoProfissional: z.string().max(2000, 'O resumo pode ter no máximo 2000 caracteres.'),
})

export type CandidatoForm = z.infer<typeof candidatoSchema>