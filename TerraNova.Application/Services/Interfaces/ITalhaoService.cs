using TerraNova.Application.DTOs;

namespace TerraNova.Application.Services.Interfaces;

public interface ITalhaoService
{
    IReadOnlyList<TalhaoResponse> GetAll();
    TalhaoResponse? GetById(Guid id);
    IReadOnlyList<TalhaoResponse> GetByPropriedadeId(Guid propriedadeId);
    IReadOnlyList<TalhaoResponse> GetByTipoPlantacaoId(Guid tipoPlantacaoId);
    TalhaoResponse Create(TalhaoRequest request);
    bool Delete(Guid id);
}