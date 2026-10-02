using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Curriculos.Api.Services;

public static class PdfTextReader
{
    // Todo PDF começa com os bytes "%PDF-"
    public static bool TemAssinaturaPdf(byte[] bytes) =>
        bytes.Length >= 5 && Encoding.ASCII.GetString(bytes, 0, 5) == "%PDF-";

    public static string LerTexto(byte[] pdf)
    {
        using var documento = PdfDocument.Open(pdf);
        var sb = new StringBuilder();
        foreach (var pagina in documento.GetPages())
            sb.AppendLine(ContentOrderTextExtractor.GetText(pagina));
        return sb.ToString();
    }
}