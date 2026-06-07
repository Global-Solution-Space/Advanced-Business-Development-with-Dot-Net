using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.DTOs;

/// <summary>
/// Dispara uma consulta a uma API externa para um talhão.
/// </summary>
public record ReqApiRequest(
    [Required] TipoParamReqApi TipoParam,
    [Required] Guid            TipoApiId,
    [Required] Guid            TalhaoId
);