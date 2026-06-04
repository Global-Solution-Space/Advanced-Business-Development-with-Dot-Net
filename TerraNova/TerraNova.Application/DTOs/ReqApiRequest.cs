using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.DTOs;

/// <summary>
/// Dispara uma consulta a uma API externa para um talhão.
/// DataInicio e DataFim são obrigatórios apenas para TipoParam = Prectotcorr (NASA POWER).
/// </summary>
public record ReqApiRequest(
    [Required] TipoParamReqApi TipoParam,
    [Required] Guid            TipoApiId,
    [Required] Guid            TalhaoId,
    [StringLength(8, MinimumLength = 8, ErrorMessage = "DataInicio deve estar no formato YYYYMMDD.")] string? DataInicio = null,
    [StringLength(8, MinimumLength = 8, ErrorMessage = "DataFim deve estar no formato YYYYMMDD.")]    string? DataFim    = null
);