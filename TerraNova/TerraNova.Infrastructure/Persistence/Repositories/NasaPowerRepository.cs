using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Repositories;

public sealed class NasaPowerRepository(TerraNovaContext context)
    : Repository<NasaPower>(context), INasaPowerRepository
{
    public IReadOnlyList<NasaPower> GetByTalhaoId(Guid talhaoId) =>
        Context.NasPowers.AsNoTracking()
            .Where(n => n.TalhaoId == talhaoId)
            .OrderByDescending(n => n.DataFim)
            .ToList();
}