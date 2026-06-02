using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.Application.Services.Implementations;

public sealed class SatvegService(
    ISatvegRepository  satvegRepository,
    ITalhaoRepository  talhaoRepository) : ISatvegService
{
    public IReadOnlyList<SatvegResponse> GetAll() =>
        satvegRepository.GetAll().Select(SatvegResponse.FromDomain).ToList();
 
    public SatvegResponse? GetById(Guid id)
    {
        var s = satvegRepository.GetById(id);
        return s is null ? null : SatvegResponse.FromDomain(s);
    }
 
    public IReadOnlyList<SatvegResponse> GetByTalhaoId(Guid talhaoId) =>
        satvegRepository.GetByTalhaoId(talhaoId).Select(SatvegResponse.FromDomain).ToList();
 
    public SatvegResponse Create(SatvegRequest request)
    {
        if (!talhaoRepository.ExistsById(request.TalhaoId))
            throw new InvalidOperationException("Talhão não encontrado.");
 
        var satveg = request.ToDomain();
        satvegRepository.Add(satveg);
        return SatvegResponse.FromDomain(satveg);
    }
 
    public bool Delete(Guid id) => satvegRepository.Delete(id);
}