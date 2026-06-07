using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

/// <summary>Tipo de plantação retornado pela API.</summary>
/// <param name="Id">Identificador do tipo de plantação.</param>
/// <param name="TipoPlant">Nome do tipo de plantação.</param>
public record TipoPlantacaoResponse(Guid Id, string TipoPlant)
{
    public static TipoPlantacaoResponse FromDomain(TipoPlantacao t) =>
        new(t.Id, t.TipoPlant);
}
