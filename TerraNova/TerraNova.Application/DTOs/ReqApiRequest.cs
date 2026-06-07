using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Enums;

namespace TerraNova.Application.DTOs;

/// <summary>
/// Dispara uma consulta a uma API externa para um talhão.
/// </summary>
/// <param name="TipoParam">Parâmetro consultado na API externa.</param>
/// <param name="TipoApiId">Identificador do tipo de API utilizado.</param>
/// <param name="TalhaoId">Identificador do talhão analisado.</param>
public record ReqApiRequest(
    [Required] TipoParamReqApi TipoParam,
    [Required] Guid            TipoApiId,
    [Required] Guid            TalhaoId
);