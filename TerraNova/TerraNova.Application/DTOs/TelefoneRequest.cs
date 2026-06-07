using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

using TerraNova.Application.DTOs.Validators;

namespace TerraNova.Application.DTOs;

[UniqueTelefone]
public record TelefoneRequest(
    [Required][StringLength(2, MinimumLength = 2, ErrorMessage = "O DDD deve ter exatamente 2 dígitos.")] string Ddd,
    [Required][StringLength(9, MinimumLength = 8, ErrorMessage = "O número deve ter entre 8 e 9 dígitos.")] string Numero,
    [Required] Guid ProdutorId)
{
    public Telefone ToDomain() => new(Ddd, Numero, ProdutorId);
}
