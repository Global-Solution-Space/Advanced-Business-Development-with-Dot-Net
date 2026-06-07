using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

/// <summary>Dados para cadastrar um talhão dentro de uma propriedade.</summary>
/// <param name="NomeTalhao">Nome do talhão.</param>
/// <param name="VolumArea">Volume ou área do talhão.</param>
/// <param name="TipoPlantacaoId">Identificador do tipo de plantação.</param>
/// <param name="PropriedadeId">Identificador da propriedade à qual o talhão pertence.</param>
/// <param name="LocalizacaoId">Identificador da localização exclusiva do talhão.</param>
public record TalhaoRequest(
    [Required][StringLength(30, MinimumLength = 2)] string  NomeTalhao,
    [Range(0.01, double.MaxValue, ErrorMessage = "O volume/área deve ser maior que zero.")] decimal VolumArea,
    [Required] Guid TipoPlantacaoId,
    [Required] Guid PropriedadeId,
    [Required] Guid LocalizacaoId)
{
    public Talhao ToDomain() => new(NomeTalhao, VolumArea, TipoPlantacaoId, PropriedadeId, LocalizacaoId);
}
