using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

/// <summary>Tipo de API externa retornado pela API.</summary>
/// <param name="Id">Identificador do tipo de API.</param>
/// <param name="NomeTipoApi">Nome do tipo de API.</param>
public record TipoApiResponse(Guid Id, string NomeTipoApi)
{
    public static TipoApiResponse FromDomain(TipoApi t) => new(t.Id, t.NomeTipoApi);
}
