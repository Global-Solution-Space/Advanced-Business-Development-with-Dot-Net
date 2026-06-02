using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record NasaPowerResponse(
    Guid    Id,
    string  DataInicio,
    string  DataFim,
    decimal Latitude,
    decimal Longitude,
    decimal Elevacao,
    Guid    TalhaoId)
{
    public static NasaPowerResponse FromDomain(NasaPower n) =>
        new(n.Id, n.DataInicio, n.DataFim, n.Latitude, n.Longitude, n.Elevacao, n.TalhaoId);
}
