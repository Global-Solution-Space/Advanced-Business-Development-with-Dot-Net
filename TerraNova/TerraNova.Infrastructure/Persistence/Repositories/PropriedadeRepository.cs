using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Repositories;

public sealed class PropriedadeRepository(TerraNovaContext context)
    : Repository<Propriedade>(context), IPropriedadeRepository
{
    public IReadOnlyList<Propriedade> GetByProdutorId(Guid produtorId) =>
        Context.Propriedades.AsNoTracking()
            .Where(p => p.ProdutorId == produtorId)
            .OrderBy(p => p.Nome)
            .ToList();

    public bool ExistsByLocalizacaoId(Guid localizacaoId) =>
        Context.Propriedades.AsNoTracking()
            .Count(p => p.LocalizacaoId == localizacaoId) > 0;
}
