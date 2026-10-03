using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Curriculos.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Curriculos.Api.Tests;

public class ApiIntegrationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static CandidatoCreateDto NovoCandidato(string email) => new()
    {
        NomeCompleto = "Maria Souza",
        Email = email,
        Telefone = "(41) 99999-1234",
        AreaInteresse = "Desenvolvimento"
    };

    private static MultipartFormDataContent Upload(byte[] bytes, string nomeArquivo)
    {
        var arquivo = new ByteArrayContent(bytes);
        arquivo.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        return new MultipartFormDataContent { { arquivo, "arquivo", nomeArquivo } };
    }

    [Fact]
    public async Task Post_DadosValidos_Retorna201EApareceNaListagemENoDetalhe()
    {
        var resposta = await _client.PostAsJsonAsync("/api/candidatos", NovoCandidato("maria@teste.com"));

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        var criado = await resposta.Content.ReadFromJsonAsync<CandidatoDto>();
        Assert.NotNull(criado);

        var lista = await _client.GetFromJsonAsync<List<CandidatoResumoDto>>("/api/candidatos");
        Assert.Contains(lista!, c => c.Id == criado.Id);

        var detalhe = await _client.GetFromJsonAsync<CandidatoDto>($"/api/candidatos/{criado.Id}");
        Assert.Equal("maria@teste.com", detalhe!.Email);
    }

    [Fact]
    public async Task Post_SemNomeEComEmailInvalido_Retorna400ComErroPorCampo()
    {
        var dto = new CandidatoCreateDto { NomeCompleto = "", Email = "abc" };

        var resposta = await _client.PostAsJsonAsync("/api/candidatos", dto);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        var problema = await resposta.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("NomeCompleto", problema!.Errors.Keys);
        Assert.Contains("Email", problema.Errors.Keys);
    }

    [Fact]
    public async Task Post_EmailJaCadastrado_Retorna409()
    {
        await _client.PostAsJsonAsync("/api/candidatos", NovoCandidato("duplicado@teste.com"));

        var resposta = await _client.PostAsJsonAsync("/api/candidatos", NovoCandidato("DUPLICADO@teste.com"));

        Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
    }

    [Fact]
    public async Task Get_IdInexistente_Retorna404()
    {
        var resposta = await _client.GetAsync("/api/candidatos/99999");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task Extrair_CurriculoFicticio_RetornaNomeEmailETelefone()
    {
        var pdf = await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "curriculo-ficticio.pdf"));

        var resposta = await _client.PostAsync("/api/curriculos/extrair", Upload(pdf, "curriculo-ficticio.pdf"));

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var dados = await resposta.Content.ReadFromJsonAsync<ExtracaoResultadoDto>();
        Assert.Equal("Ana Beatriz Lima", dados!.NomeCompleto);
        Assert.Equal("ana.lima@exemplo.com", dados.Email);
        Assert.Equal("(41) 99876-5432", dados.Telefone);
    }

    [Fact]
    public async Task Extrair_ArquivoQueNaoEhPdf_Retorna400()
    {
        var falso = Encoding.UTF8.GetBytes("isto é um texto, não um PDF");

        var resposta = await _client.PostAsync("/api/curriculos/extrair", Upload(falso, "falso.pdf"));

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Extrair_ArquivoAcimaDe5Mb_Retorna413()
    {
        var grande = new byte[5 * 1024 * 1024 + 1];

        var resposta = await _client.PostAsync("/api/curriculos/extrair", Upload(grande, "grande.pdf"));

        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, resposta.StatusCode);
    }
}