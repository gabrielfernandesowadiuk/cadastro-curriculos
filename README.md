\# Cadastro de Currículos — Desafio CIEE/PR



Aplicação para cadastrar e consultar candidatos. O cadastro pode ser feito manualmente ou a partir de um currículo em PDF: o backend extrai o texto, tenta identificar nome, e-mail e telefone, e preenche o mesmo formulário, que pode ser corrigido antes de salvar.



O relato do desenvolvimento e do uso de IA está em \[DESENVOLVIMENTO.md](DESENVOLVIMENTO.md).



\## Tecnologias e versões



| Camada | Tecnologia | Versão |

| --- | --- | --- |

| Backend | .NET SDK / ASP.NET Core | 9.0 |

| ORM | Entity Framework Core (SqlServer) | 9.0.x |

| Leitura de PDF | PdfPig | 0.1.16 |

| Documentação da API | Microsoft.AspNetCore.OpenApi + Swashbuckle SwaggerUI | 9.0.6 / 10.2.3 |

| Banco de dados | SQL Server Express | instância local |

| Testes | xUnit, Microsoft.AspNetCore.Mvc.Testing, EF Core InMemory | 2.x / 9.0.x |

| Frontend | React + TypeScript | 19.2 / 6.0 |

| Build do frontend | Vite | 8.3 |

| Formulário e validação | react-hook-form + zod (+ @hookform/resolvers) | 7.89 / 4.6 (5.9) |

| Rotas | react-router-dom | 7.18 |

| Node.js | | 24.11 |

\## Pré-requisitos



\- .NET SDK 9

\- Node.js 20 ou superior

\- SQL Server (Express, Developer ou LocalDB)

\- Ferramenta do EF Core: `dotnet tool install --global dotnet-ef --version 9.\\\*`



\## Estrutura



```

api/src/Curriculos.Api      API ASP.NET Core (controllers, DTOs, serviços, migrations)

api/tests/Curriculos.Api.Tests   Testes unitários e de integração

web/                        Frontend React

database/script.sql         Script SQL gerado a partir das migrations

docs/curriculo-ficticio.pdf Currículo fictício para testar a importação

```



\## 1. Configurar a conexão com o SQL Server



A connection string padrão está em `api/src/Curriculos.Api/appsettings.json` e usa autenticação do Windows, sem credenciais:



```

Server=localhost\\\\SQLEXPRESS;Database=CurriculosDb;Trusted\\\_Connection=True;TrustServerCertificate=True

```



Ajuste `Server` para a sua instância (`localhost` para a instância padrão, `(localdb)\\\\MSSQLLocalDB` para LocalDB).



Para usar usuário e senha, não edite o `appsettings.json`. Use user-secrets, que ficam fora do repositório (modelo em `appsettings.Example.json`):



```powershell

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=SEU\\\_SERVIDOR;Database=CurriculosDb;User Id=SEU\\\_USUARIO;Password=SUA\\\_SENHA;TrustServerCertificate=True" -p api/src/Curriculos.Api

```



\## 2. Criar a estrutura do banco



Opção A, com migrations (cria o banco e a tabela):



```powershell

dotnet ef database update -p api/src/Curriculos.Api

```



Opção B, com script SQL: crie um banco chamado `CurriculosDb` e execute `database/script.sql` nele.



\## 3. Executar a aplicação



API (a partir da raiz):



```powershell

dotnet run --project api/src/Curriculos.Api

```



\- API: http://localhost:5062

\- Swagger: http://localhost:5062/swagger



Frontend (em outro terminal):



```powershell

cd web

npm install

npm run dev

```



\- Aplicação: http://localhost:5173



Se a API rodar em outra porta, copie `web/.env.example` para `web/.env` e ajuste `VITE\\\_API\\\_URL`.



\## 4. Rodar os testes



```powershell

dotnet test

```



São 14 testes. Eles não precisam de SQL Server: os testes de integração sobem a API em memória com um banco em memória.



\## 5. Testar a importação de PDF



1\. Abra http://localhost:5173 e clique em "Novo candidato".

2\. Em "Importar de um currículo em PDF", escolha `docs/curriculo-ficticio.pdf`.

3\. Nome, e-mail e telefone são preenchidos. Complete os demais campos e salve.



\## Endpoints



| Método | Rota | Descrição |

| --- | --- | --- |

| POST | `/api/candidatos` | Cadastra um candidato |

| GET | `/api/candidatos` | Lista os candidatos |

| GET | `/api/candidatos/{id}` | Detalhes de um candidato |

| POST | `/api/curriculos/extrair` | Recebe um PDF e devolve nome, e-mail e telefone encontrados (não grava nada) |



\## Validações



\- Nome completo e e-mail obrigatórios; e-mail em formato válido e único.

\- Arquivo: somente PDF (extensão e assinatura `%PDF-`), até 5 MB.

\- As mesmas regras valem no frontend (zod) e no backend (DataAnnotations).

\- Falha na leitura do PDF não impede o cadastro manual.



\## Limitações conhecidas



\- PDFs digitalizados (imagem) não têm texto para extrair; não há OCR.

\- O nome é identificado por heurística (primeira linha que parece um nome entre as 10 primeiras). Currículos em duas colunas ou com um título antes do nome podem falhar.

\- O telefone é reconhecido apenas em formatos brasileiros.

\- Área de interesse e resumo profissional não são extraídos do PDF.

\- Não há edição nem exclusão de candidatos, nem paginação na listagem.



\## Problemas comuns



\- \*\*"Could not open a connection to SQL Server"\*\*: o nome da instância em `Server` está incorreto ou o serviço está parado.

\- \*\*"Permissão CREATE DATABASE negada"\*\*: o usuário precisa do papel `dbcreator` no SQL Server.

