using TerraNova.Domain.Entities;

namespace TerraNova.Application.Repositories;

public interface INasaPowerRepository : IRepository<NasaPower>
{
    IReadOnlyList<NasaPower> GetByTalhaoId(Guid talhaoId);
}