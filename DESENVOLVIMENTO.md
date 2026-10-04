\# Relato do desenvolvimento



\## Como organizei e executei o trabalho



Trabalhei entre 29/09 e 04/10, de 1 a 3 horas por dia. Comecei pedindo à IA um plano com cronograma, arquitetura e roteiro de commits. Depois segui passo a passo, sempre no mesmo ciclo: a IA explicava a etapa e passava os comandos e o código, eu executava na minha máquina, testava, devolvia o resultado ou o erro, e só então fazia o commit e seguia para a próxima etapa.



As etapas foram:



1\. Conferência do ambiente (.NET, Node, Git, SQL Server).

2\. Estrutura do repositório: solução .NET com API e testes, e app React com Vite.

3\. Banco de dados: entidade, DbContext, migration e script SQL.

4\. Endpoints de cadastro, listagem e detalhe, com validação.

5\. Upload do PDF e extração de nome, e-mail e telefone, com testes unitários.

6\. Frontend: formulário único, importação do PDF, listagem e detalhes.

7\. Testes de integração da API.

8\. Documentação.



\## Principais decisões técnicas



As decisões abaixo foram propostas pela IA durante o planejamento. Adotei cada uma depois de entender o motivo.



\- \*\*ASP.NET Core + React:\*\* são as tecnologias em que tenho mais experiência (C# no backend e React em projeto anterior).

\- \*\*.NET 9:\*\* era o SDK instalado na minha máquina. A IA sugeriu instalar o .NET 10 (LTS), mas mantive o 9 para não mexer no ambiente no meio do desafio.

\- \*\*SQL Server local em vez de Docker:\*\* o plano inicial previa Docker. Como eu já tinha o SQL Server Express instalado, perguntei se o Docker era necessário e decidimos usar a instância local.

\- \*\*Autenticação do Windows na connection string:\*\* assim o `appsettings.json` não contém usuário nem senha. Para quem usa login do SQL Server, o README orienta a usar user-secrets.

\- \*\*EF Core com migrations e script SQL:\*\* a migration cria a estrutura, e o `database/script.sql` (gerado a partir dela) permite criar a tabela sem o EF.

\- \*\*O endpoint de extração não grava nada:\*\* `POST /api/curriculos/extrair` só devolve os campos encontrados. O salvamento é sempre `POST /api/candidatos`. Isso garante o mesmo formulário e as mesmas regras nos dois fluxos, e uma falha no PDF não afeta o cadastro manual.

\- \*\*PdfPig para ler o PDF:\*\* biblioteca gratuita, sem dependências nativas, que preserva as quebras de linha do texto.

\- \*\*Heurísticas com regex em vez de IA na extração:\*\* é simples, previsível, fácil de testar e não depende de serviço externo.

\- \*\*Validação nas duas camadas:\*\* zod no frontend e DataAnnotations no backend, com as mesmas regras. O e-mail usa regex porque o `\[EmailAddress]` do .NET só verifica a presença do `@`.

\- \*\*E-mail único:\*\* índice único no banco e resposta 409 na API.

\- \*\*Validação do arquivo:\*\* extensão `.pdf`, assinatura `%PDF-` nos primeiros bytes e limite de 5 MB.

\- \*\*Testes de integração com banco em memória:\*\* rodam em qualquer máquina, sem SQL Server.



\## Ferramentas de IA e modelos



Usei apenas o Claude (Anthropic), pelo aplicativo desktop. Modelo configurado na sessão: `claude-opus-5-5`. Não usei nenhuma outra ferramenta de IA.



\## Em quais etapas a IA ajudou



A IA participou de todas as etapas. O plano, a arquitetura, o código do backend, do frontend e dos testes e a estrutura da documentação foram gerados por ela. Apliquei o código como foi entregue, sem alterações relevantes. Meu papel foi executar cada etapa, testar, relatar os erros e decidir quando seguir.



Exemplos de pedidos:



\- Enviei o e-mail do desafio e pedi ajuda para planejar. Resultado: um plano com cronograma, arquitetura e roteiro de commits, que segui.

\- "Estou perdido por onde começar, vamos fazer juntos passo a passo." A partir daí o trabalho passou a ser feito em etapas pequenas, com conferência a cada uma.

\- "Docker será necessário? Eu tenho o SQL Server instalado na máquina." Resultado: o Docker foi retirado do plano.

\- Colei as mensagens de erro do `dotnet ef database update`. A IA identificou a causa de cada uma e indicou a correção.

\- Perguntei por que a pasta `web` não aparecia na solução do Visual Studio. A IA explicou que a solução lista apenas os projetos .NET.



\## O que precisei corrigir, adaptar ou descartar



\- \*\*Docker:\*\* descartado, como explicado acima.

\- \*\*.NET 10:\*\* sugestão não adotada; mantive o .NET 9.

\- \*\*Caminho do projeto na migration:\*\* o comando falhou porque executei de dentro da pasta da API, e o caminho relativo ficou duplicado. Passei a rodar da raiz.

\- \*\*Erro 40 (servidor não encontrado):\*\* a connection string usava `localhost`, mas minha instância é nomeada. Corrigi para `localhost\\SQLEXPRESS`.

\- \*\*Erro 18456 (falha de login do `sa`):\*\* a tentativa com usuário e senha não funcionou. Troquei para autenticação do Windows e removi o user-secret.

\- \*\*Erro 262 (permissão CREATE DATABASE negada):\*\* meu usuário do Windows não tinha permissão para criar bancos. Concedi o papel `dbcreator` pelo SSMS.

\- \*\*Nome de arquivo com maiúscula errada:\*\* salvei `ListaPAge.tsx` em vez de `ListaPage.tsx`. No Windows funcionava, mas o TypeScript acusou o erro, e em Linux o projeto quebraria. Renomeei.

\- \*\*Instrução incorreta da IA:\*\* ao gerar o README, ela me orientou a trocar um marcador em uma URL que não existia no texto. Questionei e ela reconheceu o erro.



\## Como verifiquei a solução



\- \*\*Swagger:\*\* cadastro válido (201), e-mail repetido (409), nome vazio e e-mail inválido (400 com mensagem por campo), id inexistente (404).

\- \*\*Upload pelo Swagger:\*\* PDF fictício (campos extraídos), arquivo de texto renomeado para `.pdf` (400) e imagem (400).

\- \*\*Navegador:\*\* cadastro manual, cadastro com PDF, listagem, detalhes, envio com campos vazios e e-mail duplicado.

\- \*\*Testes automatizados:\*\* `dotnet test`, com 14 testes passando (7 unitários do extrator e 7 de integração da API).

\- \*\*Build do frontend:\*\* `npm run build` sem erros de TypeScript.



\## Tempo dedicado



Entre 1 e 3 horas por dia, de 29/09 a 04/10. No total, aproximadamente 10 a 12 horas.



\## Dificuldades



\- A maior dificuldade foi a configuração do SQL Server: nome da instância, tipo de autenticação e permissão para criar o banco. Foram três erros seguidos antes da primeira migration funcionar.

\- Tive dúvidas sobre a organização das pastas, como a diferença entre a raiz do repositório, a solução .NET e o projeto web, e sobre em qual pasta rodar cada comando.

\- Como o código foi gerado pela IA, o ponto que mais exige de mim é estudar cada parte para explicá-la e evoluí-la.



\## Limitações



\- PDFs digitalizados não são lidos, porque não há OCR.

\- O nome é identificado por heurística e pode falhar em currículos com duas colunas ou com um título antes do nome.

\- O telefone é reconhecido apenas em formatos brasileiros.

\- Área de interesse e resumo profissional não são extraídos do PDF.

\- Os testes de integração usam banco em memória, que não valida o índice único nem o tamanho das colunas como o SQL Server.

\- Não há testes automatizados no frontend.



\## O que faria com mais tempo



\- `docker-compose` com SQL Server, para facilitar a execução em qualquer máquina.

\- Testes de integração contra um SQL Server real (Testcontainers) e testes do formulário no frontend.

\- OCR para PDFs digitalizados e extração de mais campos.

\- Edição e exclusão de candidatos, busca e paginação na listagem.

\- Pipeline de CI rodando build e testes a cada push.

