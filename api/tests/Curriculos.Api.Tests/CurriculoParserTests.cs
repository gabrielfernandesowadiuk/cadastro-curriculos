using Curriculos.Api.Services;

namespace Curriculos.Api.Tests;

public class CurriculoParserTests
{
    private const string TextoCompleto = """
        Currículo
        Ana Beatriz Lima
        Curitiba - PR
        E-mail: Ana.Lima@exemplo.com | Telefone: (41) 99876-5432
        Resumo: Desenvolvedora .NET
        """;

    [Fact]
    public void Extrair_TextoCompleto_RetornaNomeEmailETelefone()
    {
        var r = CurriculoParser.Extrair(TextoCompleto);

        Assert.Equal("Ana Beatriz Lima", r.NomeCompleto);
        Assert.Equal("ana.lima@exemplo.com", r.Email);
        Assert.Equal("(41) 99876-5432", r.Telefone);
    }

    [Theory]
    [InlineData("(41) 99876-5432")]
    [InlineData("41998765432")]
    [InlineData("+55 41 3333-4444")]
    public void ExtrairTelefone_FormatosBrasileiros_Reconhece(string telefone) =>
        Assert.Equal(telefone, CurriculoParser.ExtrairTelefone($"Contato: {telefone}"));

    [Fact]
    public void ExtrairNome_PrimeiraLinhaEhTitulo_IgnoraTitulo() =>
        Assert.Equal("João da Silva", CurriculoParser.ExtrairNome("Curriculum Vitae\nJoão da Silva"));

    [Fact]
    public void ExtrairNome_LinhaComCidadeEUf_NaoConfundeComNome() =>
        Assert.Null(CurriculoParser.ExtrairNome("Curitiba - PR"));

    [Fact]
    public void Extrair_TextoSemDados_RetornaNulos()
    {
        var r = CurriculoParser.Extrair("Documento 2024 sem contato");

        Assert.Null(r.NomeCompleto);
        Assert.Null(r.Email);
        Assert.Null(r.Telefone);
    }
}