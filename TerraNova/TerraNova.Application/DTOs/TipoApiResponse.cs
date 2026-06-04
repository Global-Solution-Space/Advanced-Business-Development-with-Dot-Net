using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record TipoApiResponse(Guid Id, string NomeTipoApi)
{
    public static TipoApiResponse FromDomain(TipoApi t) => new(t.Id, t.NomeTipoApi);
}
