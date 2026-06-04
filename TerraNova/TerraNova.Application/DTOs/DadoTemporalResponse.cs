using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record DadoTemporalResponse(
    Guid     Id,
    DateTime DataLeitura,
    decimal  Valor,
    Guid     TalhaoId,
    Guid     ReqApiId)
{
    public static DadoTemporalResponse FromDomain(DadoTemporal d) =>
        new(d.Id, d.DataLeitura, d.Valor, d.TalhaoId, d.ReqApiId);
}