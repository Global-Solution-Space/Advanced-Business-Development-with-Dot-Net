using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

/// <summary>Dado temporal coletado de uma API externa para um talhão.</summary>
/// <param name="Id">Identificador do dado temporal.</param>
/// <param name="DataLeitura">Data da leitura informada pela fonte externa.</param>
/// <param name="Valor">Valor numérico coletado.</param>
/// <param name="TalhaoId">Identificador do talhão relacionado.</param>
/// <param name="ReqApiId">Identificador da requisição de API que originou o dado.</param>
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