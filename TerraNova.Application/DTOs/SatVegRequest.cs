using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record SatvegRequest(
    [Range(0, int.MaxValue)] int      TipoPerfil,
    [Range(0, int.MaxValue)] int      Satelite,
    bool?                             PreFiltro,
    [StringLength(3)]        string?  Filtro,
    [Range(0, 99)]           int?     ParametroFiltro,
    [Required]               string   Poligono,
    bool                             TodasEstatisticas,
    [Required]               DateTime DataAnalise,
    [Required]               Guid     TalhaoId)
{
    public Satveg ToDomain() =>
        new(TipoPerfil, Satelite, PreFiltro, Filtro, ParametroFiltro,
            Poligono, TodasEstatisticas, DataAnalise, TalhaoId);
}
