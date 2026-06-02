using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Repositories;

public sealed class TalhaoRepository(TerraNovaContext context)
    : Repository<Talhao>(context), ITalhaoRepository
{
    public IReadOnlyList<Talhao> GetByPropriedadeId(Guid propriedadeId) =>
        Context.Talhoes.AsNoTracking()
            .Where(t => t.PropriedadeId == propriedadeId)
            .OrderBy(t => t.NomeTalhao)
            .ToList();
 
    public IReadOnlyList<Talhao> GetByTipoPlantacaoId(Guid tipoPlantacaoId) =>
        Context.Talhoes.AsNoTracking()
            .Where(t => t.TipoPlantacaoId == tipoPlantacaoId)
            .OrderBy(t => t.NomeTalhao)
            .ToList();
}