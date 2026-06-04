using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.Application.Services.Implementations;

public sealed class DadoTemporalService(IDadoTemporalRepository dadoTemporalRepository) : IDadoTemporalService
{
    public IReadOnlyList<DadoTemporalResponse> GetAll() =>
        dadoTemporalRepository.GetAll().Select(DadoTemporalResponse.FromDomain).ToList();
 
    public DadoTemporalResponse? GetById(Guid id)
    {
        var d = dadoTemporalRepository.GetById(id);
        return d is null ? null : DadoTemporalResponse.FromDomain(d);
    }
 
    public IReadOnlyList<DadoTemporalResponse> GetByTalhaoId(Guid talhaoId) =>
        dadoTemporalRepository.GetByTalhaoId(talhaoId).Select(DadoTemporalResponse.FromDomain).ToList();
 
    public IReadOnlyList<DadoTemporalResponse> GetByReqApiId(Guid reqApiId) =>
        dadoTemporalRepository.GetByReqApiId(reqApiId).Select(DadoTemporalResponse.FromDomain).ToList();
}
