using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record TipoPlantacaoResponse(Guid Id, string TipoPlant)
{
    public static TipoPlantacaoResponse FromDomain(TipoPlantacao t) =>
        new(t.Id, t.TipoPlant);
}
