using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record TalhaoRequest(
    [Required][StringLength(30, MinimumLength = 2)] string  NomeTalhao,
    [Range(0.01, double.MaxValue, ErrorMessage = "O volume/área deve ser maior que zero.")] decimal VolumArea,
    [Required] Guid TipoPlantacaoId,
    [Required] Guid PropriedadeId,
    [Required] Guid LocalizacaoId)
{
    public Talhao ToDomain() => new(NomeTalhao, VolumArea, TipoPlantacaoId, PropriedadeId, LocalizacaoId);
}
