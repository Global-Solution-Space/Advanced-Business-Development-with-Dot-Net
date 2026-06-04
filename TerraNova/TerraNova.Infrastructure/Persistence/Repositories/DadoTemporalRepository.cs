using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Entities;
 
namespace TerraNova.Infrastructure.Persistence.Repositories;
 
public sealed class DadoTemporalRepository(TerraNovaContext context)
    : Repository<DadoTemporal>(context), IDadoTemporalRepository
{
    public IReadOnlyList<DadoTemporal> GetByTalhaoId(Guid talhaoId) =>
        Context.DadosTemporais.AsNoTracking()
            .Where(d => d.TalhaoId == talhaoId)
            .OrderBy(d => d.DataLeitura)
            .ToList();
 
    public IReadOnlyList<DadoTemporal> GetByReqApiId(Guid reqApiId) =>
        Context.DadosTemporais.AsNoTracking()
            .Where(d => d.ReqApiId == reqApiId)
            .OrderBy(d => d.DataLeitura)
            .ToList();
 
    public IReadOnlyList<DadoTemporal> GetByTalhaoAndReqApi(Guid talhaoId, Guid reqApiId) =>
        Context.DadosTemporais.AsNoTracking()
            .Where(d => d.TalhaoId == talhaoId && d.ReqApiId == reqApiId)
            .OrderBy(d => d.DataLeitura)
            .ToList();
 
    public void AddRange(IEnumerable<DadoTemporal> dados)
    {
        Context.DadosTemporais.AddRange(dados);
        Context.SaveChanges();
    }
}