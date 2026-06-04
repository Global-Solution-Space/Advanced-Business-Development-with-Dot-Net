using TerraNova.Domain.Entities;

namespace TerraNova.Application.Repositories;

public interface IDadoTemporalRepository : IRepository<DadoTemporal>
{
    IReadOnlyList<DadoTemporal> GetByTalhaoId(Guid talhaoId);
    IReadOnlyList<DadoTemporal> GetByReqApiId(Guid reqApiId);
    IReadOnlyList<DadoTemporal> GetByTalhaoAndReqApi(Guid talhaoId, Guid reqApiId);
    void AddRange(IEnumerable<DadoTemporal> dados);
}