using Curriculos.Api.Data;
using Curriculos.Api.Dtos;
using Curriculos.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Curriculos.Api.Controllers;

[ApiController]
[Route("api/candidatos")]
public class CandidatosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<CandidatoResumoDto>> Listar() =>
        await db.Candidatos.AsNoTracking()
            .OrderByDescending(c => c.CriadoEm)
            .Select(c => new CandidatoResumoDto(c.Id, c.NomeCompleto, c.Email, c.AreaInteresse, c.CriadoEm))
            .ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CandidatoDto>> ObterPorId(int id)
    {
        var candidato = await db.Candidatos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return candidato is null
            ? Problem(statusCode: 404, title: "Candidato não encontrado.")
            : CandidatoDto.De(candidato);
    }

    [HttpPost]
    public async Task<ActionResult<CandidatoDto>> Criar(CandidatoCreateDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await db.Candidatos.AnyAsync(c => c.Email == email))
            return Problem(statusCode: 409, title: "Já existe um candidato cadastrado com este e-mail.");

        var candidato = new Candidato
        {
            NomeCompleto = dto.NomeCompleto.Trim(),
            Email = email,
            Telefone = Limpar(dto.Telefone),
            AreaInteresse = Limpar(dto.AreaInteresse),
            ResumoProfissional = Limpar(dto.ResumoProfissional)
        };

        db.Candidatos.Add(candidato);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = candidato.Id }, CandidatoDto.De(candidato));
    }

    private static string? Limpar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}