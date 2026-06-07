using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

/// <summary>Talhão retornado pela API.</summary>
/// <param name="Id">Identificador do talhão.</param>
/// <param name="NomeTalhao">Nome do talhão.</param>
/// <param name="VolumArea">Volume ou área do talhão.</param>
/// <param name="TipoPlantacaoId">Identificador do tipo de plantação.</param>
/// <param name="PropriedadeId">Identificador da propriedade vinculada.</param>
/// <param name="LocalizacaoId">Identificador da localização vinculada.</param>
public record TalhaoResponse(
    Guid    Id,
    string  NomeTalhao,
    decimal VolumArea,
    Guid    TipoPlantacaoId,
    Guid    PropriedadeId,
    Guid    LocalizacaoId)
{
    public static TalhaoResponse FromDomain(Talhao t) =>
        new(t.Id, t.NomeTalhao, t.VolumArea, t.TipoPlantacaoId, t.PropriedadeId, t.LocalizacaoId);
}
