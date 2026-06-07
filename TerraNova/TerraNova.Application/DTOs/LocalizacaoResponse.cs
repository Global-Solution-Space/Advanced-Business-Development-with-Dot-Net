using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

/// <summary>Localização geográfica retornada pela API.</summary>
/// <param name="Id">Identificador da localização.</param>
/// <param name="Latitude">Latitude em graus decimais.</param>
/// <param name="Longitude">Longitude em graus decimais.</param>
public record LocalizacaoResponse(Guid Id, decimal Latitude, decimal Longitude)
{
    public static LocalizacaoResponse FromDomain(Localizacao l) =>
        new(l.Id, (decimal)l.Coordenadas.Y, (decimal)l.Coordenadas.X);
}
