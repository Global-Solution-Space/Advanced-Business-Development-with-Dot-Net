using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

using TerraNova.Application.DTOs.Validators;

namespace TerraNova.Application.DTOs;

/// <summary>Dados para cadastrar ou atualizar um telefone detalhado vinculado a um produtor.</summary>
/// <param name="Ddd">DDD do telefone, com dois dígitos.</param>
/// <param name="Numero">Número de telefone, com oito ou nove dígitos.</param>
/// <param name="ProdutorId">Identificador do produtor dono do telefone.</param>
[UniqueTelefone]
public record TelefoneRequest(
    [Required]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "O DDD deve ter exatamente 2 dígitos.")]
    [RegularExpression(@"^\d{2}$", ErrorMessage = "O DDD deve conter apenas dígitos.")]
    string Ddd,

    [Required]
    [StringLength(9, MinimumLength = 8, ErrorMessage = "O número deve ter entre 8 e 9 dígitos.")]
    [RegularExpression(@"^\d{8,9}$", ErrorMessage = "O número deve conter apenas dígitos.")]
    string Numero,

    [Required] Guid ProdutorId)
{
    public Telefone ToDomain() => new(Ddd, Numero, ProdutorId);
}
