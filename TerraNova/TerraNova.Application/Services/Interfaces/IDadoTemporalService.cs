using TerraNova.Application.DTOs;

namespace TerraNova.Application.Services.Interfaces;

public interface IDadoTemporalService
{
    IReadOnlyList<DadoTemporalResponse> GetAll();
    DadoTemporalResponse? GetById(Guid id);
    IReadOnlyList<DadoTemporalResponse> GetByTalhaoId(Guid talhaoId);
    IReadOnlyList<DadoTemporalResponse> GetByReqApiId(Guid reqApiId);
}