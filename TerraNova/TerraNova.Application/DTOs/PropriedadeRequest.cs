using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record PropriedadeRequest(
    [Required][StringLength(30, MinimumLength = 2)] string  Nome,
    [Range(0.01, double.MaxValue, ErrorMessage = "O tamanho total deve ser maior que zero.")] decimal TamanhoTotal,
    [Required] Guid ProdutorId,
    [Required] Guid LocalizacaoId)
{
    public Propriedade ToDomain() => new(Nome, TamanhoTotal, ProdutorId, LocalizacaoId);
}
