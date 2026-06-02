using TerraNova.Domain.Entities;

namespace TerraNova.Application.Repositories;

public interface ISatvegRepository : IRepository<Satveg>
{
    IReadOnlyList<Satveg> GetByTalhaoId(Guid talhaoId);
}