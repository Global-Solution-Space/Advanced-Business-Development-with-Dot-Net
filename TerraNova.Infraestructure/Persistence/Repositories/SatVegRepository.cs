using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Entities;

namespace TerraNova.Infraestructure.Persistence.Repositories;

public sealed class SatvegRepository(TerraNovaContext context)
    : Repository<Satveg>(context), ISatvegRepository
{
    public IReadOnlyList<Satveg> GetByTalhaoId(Guid talhaoId) =>
        Context.Satvegs.AsNoTracking()
            .Where(s => s.TalhaoId == talhaoId)
            .OrderByDescending(s => s.DataAnalise)
            .ToList();
}