using Curriculos.Api.Dtos;
using Curriculos.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Curriculos.Api.Controllers;

[ApiController]
[Route("api/curriculos")]
public class CurriculosController : ControllerBase
{
    private const long TamanhoMaximo = 5 * 1024 * 1024; // 5 MB

    [HttpPost("extrair")]
    [RequestSizeLimit(10 * 1024 * 1024)] // folga para conseguirmos responder com mensagem própria
    public async Task<ActionResult<ExtracaoResultadoDto>> Extrair(IFormFile? arquivo)
    {
        if (arquivo is null || arquivo.Length == 0)
            return Problem(statusCode: 400, title: "Nenhum arquivo foi enviado.");

        if (arquivo.Length > TamanhoMaximo)
            return Problem(statusCode: 413, title: "O arquivo excede o limite de 5 MB.");

        if (!Path.GetExtension(arquivo.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            return Problem(statusCode: 400, title: "Formato inválido. Envie um arquivo PDF.");

        using var ms = new MemoryStream();
        await arquivo.CopyToAsync(ms);
        var bytes = ms.ToArray();

        if (!PdfTextReader.TemAssinaturaPdf(bytes))
            return Problem(statusCode: 400, title: "O arquivo enviado não é um PDF válido.");

        string texto;
        try
        {
            texto = PdfTextReader.LerTexto(bytes);
        }
        catch (Exception)
        {
            return Problem(statusCode: 422,
                title: "Não foi possível ler o PDF. Preencha os dados manualmente.");
        }

        if (string.IsNullOrWhiteSpace(texto))
            return Problem(statusCode: 422,
                title: "O PDF não contém texto legível (pode ser uma imagem digitalizada). Preencha os dados manualmente.");

        return CurriculoParser.Extrair(texto);
    }
}