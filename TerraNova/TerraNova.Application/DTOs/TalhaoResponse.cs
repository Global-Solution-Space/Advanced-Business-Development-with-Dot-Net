using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

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
