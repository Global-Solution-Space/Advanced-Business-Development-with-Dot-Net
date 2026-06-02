using TerraNova.Application.DTOs;

namespace TerraNova.Application.Services.Interfaces;

public interface INasaPowerService
{
    IReadOnlyList<NasaPowerResponse> GetAll();
    NasaPowerResponse? GetById(Guid id);
    IReadOnlyList<NasaPowerResponse> GetByTalhaoId(Guid talhaoId);
    NasaPowerResponse Create(NasaPowerRequest request);
    bool Delete(Guid id);
}