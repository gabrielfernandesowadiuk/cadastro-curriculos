using System.Text.RegularExpressions;
using Curriculos.Api.Dtos;

namespace Curriculos.Api.Services;

public static class CurriculoParser
{
    private static readonly Regex EmailRegex =
        new(@"[\w.+-]+@[\w-]+(\.[\w-]+)+", RegexOptions.Compiled);

    // Formatos brasileiros: (41) 99999-1234, 41999991234, +55 41 3333-4444
    private static readonly Regex TelefoneRegex =
        new(@"(?<!\d)(\+?55[\s.-]?)?\(?\d{2}\)?[\s.-]?9?\d{4}[\s.-]?\d{4}(?!\d)", RegexOptions.Compiled);

    private static readonly string[] PalavrasIgnoradas =
        ["curriculo", "currículo", "curriculum", "vitae", "resume", "cv"];

    public static ExtracaoResultadoDto Extrair(string texto) =>
        new(ExtrairNome(texto), ExtrairEmail(texto), ExtrairTelefone(texto));

    public static string? ExtrairEmail(string texto)
    {
        var m = EmailRegex.Match(texto);
        return m.Success ? m.Value.ToLowerInvariant() : null;
    }

    public static string? ExtrairTelefone(string texto)
    {
        var m = TelefoneRegex.Match(texto);
        return m.Success ? m.Value.Trim() : null;
    }

    // Primeira linha (entre as 10 primeiras) que "parece" um nome:
    // de 2 a 6 palavras, só letras, sem dígitos nem @, e que não seja o título "Currículo".
    public static string? ExtrairNome(string texto)
    {
        var linhas = texto.Split('\n')
            .Select(l => l.Trim())
            .Where(l => l.Length > 0)
            .Take(10);

        foreach (var linha in linhas)
        {
            if (linha.Contains('@') || linha.Any(char.IsDigit)) continue;

            var palavras = linha.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (palavras.Length is < 2 or > 6) continue;
            if (palavras.Any(p => PalavrasIgnoradas.Contains(p.ToLowerInvariant()))) continue;
            if (!palavras.All(EhPalavraDeNome)) continue;

            return linha;
        }
        return null;
    }

    private static bool EhPalavraDeNome(string p) =>
        char.IsLetter(p[0]) && p.All(c => char.IsLetter(c) || c is '\'' or '-' or '.');
}