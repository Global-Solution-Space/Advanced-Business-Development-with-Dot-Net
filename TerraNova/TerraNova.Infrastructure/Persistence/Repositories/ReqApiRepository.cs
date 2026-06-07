using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;
 
namespace TerraNova.Infrastructure.Persistence.Repositories;
 
public sealed class ReqApiRepository(TerraNovaContext context)
    : Repository<ReqApi>(context), IReqApiRepository
{
    public IReadOnlyList<ReqApi> GetByTipoParam(TipoParamReqApi tipoParam) =>
        Context.ReqApis.AsNoTracking()
            .Where(r => r.TipoParam == tipoParam)
            .OrderByDescending(r => r.DataAnalise)
            .ToList();
 
    public IReadOnlyList<ReqApi> GetByTipoApiId(Guid tipoApiId) =>
        Context.ReqApis.AsNoTracking()
            .Where(r => r.TipoApiId == tipoApiId)
            .OrderByDescending(r => r.DataAnalise)
            .ToList();

    public IReadOnlyList<ReqApi> GetByTalhaoId(Guid talhaoId) =>
        Context.ReqApis.AsNoTracking()
            .Where(r => r.DadosTemporais.Any(d => d.TalhaoId == talhaoId))
            .ToList();

    public int CountDadosByReqApiId(Guid reqApiId) =>
        Context.DadosTemporais.Count(d => d.ReqApiId == reqApiId);

    public Dictionary<Guid, int> CountDadosByReqApiIds(IEnumerable<Guid> reqApiIds)
    {
        var ids = reqApiIds.ToList();
        return Context.DadosTemporais
            .Where(d => ids.Contains(d.ReqApiId))
            .GroupBy(d => d.ReqApiId)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
