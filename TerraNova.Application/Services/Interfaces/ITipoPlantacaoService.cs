using TerraNova.Application.DTOs;

namespace TerraNova.Application.Services.Interfaces;

public interface ITipoPlantacaoService
{
    IReadOnlyList<TipoPlantacaoResponse> GetAll();
    TipoPlantacaoResponse? GetById(Guid id);
    TipoPlantacaoResponse Create(TipoPlantacaoRequest request);
    bool Delete(Guid id);
}