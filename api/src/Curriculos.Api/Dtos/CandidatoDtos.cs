using System.ComponentModel.DataAnnotations;
using Curriculos.Api.Models;

namespace Curriculos.Api.Dtos;

public class CandidatoCreateDto
{
    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome pode ter no máximo 150 caracteres.")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(150, ErrorMessage = "O e-mail pode ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O telefone pode ter no máximo 20 caracteres.")]
    public string? Telefone { get; set; }

    [StringLength(100, ErrorMessage = "A área pode ter no máximo 100 caracteres.")]
    public string? AreaInteresse { get; set; }

    [StringLength(2000, ErrorMessage = "O resumo pode ter no máximo 2000 caracteres.")]
    public string? ResumoProfissional { get; set; }
}

public record CandidatoResumoDto(int Id, string NomeCompleto, string Email, string? AreaInteresse, DateTime CriadoEm);

public record CandidatoDto(int Id, string NomeCompleto, string Email, string? Telefone,
                           string? AreaInteresse, string? ResumoProfissional, DateTime CriadoEm)
{
    public static CandidatoDto De(Candidato c) =>
        new(c.Id, c.NomeCompleto, c.Email, c.Telefone, c.AreaInteresse, c.ResumoProfissional, c.CriadoEm);
}