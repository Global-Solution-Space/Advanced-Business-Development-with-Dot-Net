using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.Application.Services.Implementations;

public sealed class NasaPowerService(
    INasaPowerRepository nasaPowerRepository,
    ITalhaoRepository    talhaoRepository) : INasaPowerService
{
    public IReadOnlyList<NasaPowerResponse> GetAll() =>
        nasaPowerRepository.GetAll().Select(NasaPowerResponse.FromDomain).ToList();
 
    public NasaPowerResponse? GetById(Guid id)
    {
        var n = nasaPowerRepository.GetById(id);
        return n is null ? null : NasaPowerResponse.FromDomain(n);
    }
 
    public IReadOnlyList<NasaPowerResponse> GetByTalhaoId(Guid talhaoId) =>
        nasaPowerRepository.GetByTalhaoId(talhaoId).Select(NasaPowerResponse.FromDomain).ToList();
 
    public NasaPowerResponse Create(NasaPowerRequest request)
    {
        if (!talhaoRepository.ExistsById(request.TalhaoId))
            throw new InvalidOperationException("Talhão não encontrado.");
 
        var nasaPower = request.ToDomain();
        nasaPowerRepository.Add(nasaPower);
        return NasaPowerResponse.FromDomain(nasaPower);
    }
 
    public bool Delete(Guid id) => nasaPowerRepository.Delete(id);
}