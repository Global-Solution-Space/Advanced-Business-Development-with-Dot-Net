using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record SatvegResponse(
    Guid     Id,
    int      TipoPerfil,
    int      Satelite,
    bool?    PreFiltro,
    string?  Filtro,
    int?     ParametroFiltro,
    string   Poligono,
    bool     TodasEstatisticas,
    DateTime DataAnalise,
    Guid     TalhaoId)
{
    public static SatvegResponse FromDomain(Satveg s) =>
        new(s.Id, s.TipoPerfil, s.Satelite, s.PreFiltro, s.Filtro,
            s.ParametroFiltro, s.Poligono, s.TodasEstatisticas, s.DataAnalise, s.TalhaoId);
}