using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record LocalizacaoResponse(Guid Id, decimal Latitude, decimal Longitude)
{
    public static LocalizacaoResponse FromDomain(Localizacao l) =>
        new(l.Id, l.Latitude, l.Longitude);
}
